using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class EstimacionService : IEstimacionService
{
    private readonly IEstimacionRepository _estimacionRepo;
    private readonly FepiDbContext _context; // para operaciones que cruzan varias tablas (conceptos, observaciones, historial)
    private readonly IAlertaService _alertaService;

    public EstimacionService(IEstimacionRepository estimacionRepo, FepiDbContext context, IAlertaService alertaService)
    {
        _estimacionRepo = estimacionRepo;
        _context = context;
        _alertaService = alertaService;
    }

    public async Task<EstimacionResumenDto> CrearAsync(CrearEstimacionDto dto, CancellationToken ct = default)
    {
        var ultimoCorrelativo = await _estimacionRepo.GetUltimoCorrelativoAsync(dto.ContratoId, ct);

        var estimacion = new Estimacion
        {
            ContratoId = dto.ContratoId,
            NumeroCorrelativo = ultimoCorrelativo + 1,
            Periodo = dto.Periodo,
            Estado = EstadoEstimacion.Borrador
        };

        await _estimacionRepo.AddAsync(estimacion, ct);
        await _estimacionRepo.SaveChangesAsync(ct);

        return new EstimacionResumenDto(estimacion.Id, estimacion.NumeroCorrelativo, estimacion.Periodo, estimacion.Estado, 0);
    }

    public async Task<List<EstimacionResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var estimaciones = await _estimacionRepo.GetByContratoAsync(contratoId, ct);
        return estimaciones.Select(e => new EstimacionResumenDto(
            e.Id, e.NumeroCorrelativo, e.Periodo, e.Estado, e.Conceptos.Sum(c => c.Importe))).ToList();
    }

    public async Task<EstimacionDetalleDto> ObtenerDetalleAsync(int estimacionId, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetConDetalleAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        var montoAcumulado = await _context.Estimaciones
            .Where(e => e.ContratoId == estimacion.ContratoId &&
                        (e.Estado == EstadoEstimacion.Aprobada || e.Estado == EstadoEstimacion.Pagada))
            .Include(e => e.Conceptos)
            .SelectMany(e => e.Conceptos)
            .SumAsync(c => c.Importe, ct);

        var montoContratado = estimacion.Contrato!.MontoContratado;
        var montoEstimado = estimacion.Conceptos.Sum(c => c.Importe);

        return new EstimacionDetalleDto(
            estimacion.Id, estimacion.ContratoId, estimacion.NumeroCorrelativo, estimacion.Periodo, estimacion.Estado,
            montoEstimado, montoContratado, montoAcumulado, montoContratado - montoAcumulado,
            estimacion.Conceptos.Select(c => new EstimacionConceptoDetalleDto(
                c.ConceptoContrato!.Clave, c.ConceptoContrato.Descripcion, c.CantidadEjecutada, c.ConceptoContrato.PrecioUnitario, c.Importe)).ToList(),
            estimacion.NotasVinculadas.Select(n => $"Folio {n.NotaBitacora!.Folio} — {n.NotaBitacora.Asunto}").ToList(),
            estimacion.Observaciones.Select(o => new ObservacionDto(o.Id, o.Texto, o.FechaCreacion, o.UsuarioId)).ToList()
        );
    }

    public async Task ActualizarConceptosAsync(int estimacionId, ActualizarConceptosEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _context.Estimaciones.Include(e => e.Conceptos).FirstOrDefaultAsync(e => e.Id == estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        _context.EstimacionConceptos.RemoveRange(estimacion.Conceptos);

        foreach (var item in dto.Conceptos)
        {
            var ConceptoContrato = await _context.ConceptosContrato.FindAsync(new object[] { item.ConceptoContratoId }, ct)
                ?? throw new InvalidOperationException($"Concepto de catálogo {item.ConceptoContratoId} no encontrado.");

            estimacion.Conceptos.Add(new EstimacionConcepto
            {
                EstimacionId = estimacionId, ConceptoContratoId = item.ConceptoContratoId,
                CantidadEjecutada = item.CantidadEjecutada, Importe = item.CantidadEjecutada * ConceptoContrato.PrecioUnitario
            });
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task VincularNotasBitacoraAsync(int estimacionId, VincularNotasDto dto, CancellationToken ct = default)
    {
        foreach (var notaId in dto.NotaBitacoraIds)
        {
            _context.EstimacionNotasBitacora.Add(new EstimacionNotaBitacora
            {
                EstimacionId = estimacionId, NotaBitacoraId = notaId
            });
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task EnviarAsync(int estimacionId, int usuarioId, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetByIdAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        var estadoAnterior = estimacion.Estado;
        estimacion.Estado = EstadoEstimacion.Enviada;
        estimacion.FechaEnvio = DateTime.UtcNow;
        estimacion.UsuarioEnvioId = usuarioId;

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId, EstadoAnterior = estadoAnterior,
            EstadoNuevo = EstadoEstimacion.Enviada, Fecha = estimacion.FechaEnvio.Value, UsuarioId = usuarioId
        });

        await _context.SaveChangesAsync(ct);

        // El catálogo describe el panel de alertas en SV-06; la estimación enviada
        // simplemente queda disponible para que supervisión la revise.
        await _alertaService.EmitirAsync(estimacion.ContratoId, TipoAlerta.EstimacionPlazoRevision, estimacion.Id,
            nameof(Estimacion), RolSistema.SupervisorExterno, $"Estimación N.° {estimacion.NumeroCorrelativo} enviada para revisión.", ct);
    }

    public async Task<ObservacionDto> AgregarObservacionAsync(int estimacionId, CrearObservacionDto dto, int usuarioId, CancellationToken ct = default)
    {
        var observacion = new EstimacionObservacion
        {
            EstimacionId = estimacionId, Texto = dto.Texto, UsuarioId = usuarioId
        };

        _context.EstimacionObservaciones.Add(observacion);
        await _context.SaveChangesAsync(ct);

        return new ObservacionDto(observacion.Id, observacion.Texto, observacion.FechaCreacion, observacion.UsuarioId);
    }

    public async Task CambiarEstadoAsync(int estimacionId, CambiarEstadoEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetByIdAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        var estadoAnterior = estimacion.Estado;
        estimacion.Estado = dto.NuevoEstado;

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId, EstadoAnterior = estadoAnterior,
            EstadoNuevo = dto.NuevoEstado, Fecha = DateTime.UtcNow, UsuarioId = dto.UsuarioId, Comentario = dto.Comentario
        });

        await _context.SaveChangesAsync(ct);

        if (dto.NuevoEstado != EstadoEstimacion.Observada)
            await _alertaService.ResolverAsync(TipoAlerta.EstimacionPlazoRevision, estimacion.Id, ct);
    }

    public async Task RegistrarPagoAsync(int estimacionId, RegistrarPagoEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetByIdAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        _context.EstimacionPagos.Add(new EstimacionPago
        {
            EstimacionId = estimacionId, FechaPago = dto.FechaPago,
            ReferenciaBancaria = dto.ReferenciaBancaria, MontoPagado = dto.MontoPagado
        });

        var estadoAnterior = estimacion.Estado;
        estimacion.Estado = EstadoEstimacion.Pagada;

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId, EstadoAnterior = estadoAnterior,
            EstadoNuevo = EstadoEstimacion.Pagada, Fecha = DateTime.UtcNow, UsuarioId = estimacion.UsuarioEnvioId ?? 0
        });

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<EstimacionHistorialDto>> ObtenerHistorialAsync(int estimacionId, CancellationToken ct = default)
    {
        var historial = await _context.EstimacionHistoriales
            .Where(h => h.EstimacionId == estimacionId)
            .OrderBy(h => h.Fecha)
            .ToListAsync(ct);

        return historial.Select(h => new EstimacionHistorialDto(h.EstadoAnterior, h.EstadoNuevo, h.Fecha, h.UsuarioId, h.Comentario)).ToList();
    }
}
