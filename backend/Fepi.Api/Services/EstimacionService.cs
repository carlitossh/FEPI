using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class EstimacionService : IEstimacionService
{
    private readonly IEstimacionRepository _estimacionRepo;
    private readonly FepiDbContext _context;
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

        return new EstimacionResumenDto(estimacion.Id, estimacion.NumeroCorrelativo, estimacion.Periodo,
            estimacion.Estado, estimacion.EstadoPago, 0, 0, 0);
    }

    public async Task<List<EstimacionResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var estimaciones = await _estimacionRepo.GetByContratoAsync(contratoId, ct);
        return estimaciones.Select(e =>
        {
            var montoEst = e.Conceptos.Sum(c => c.Importe);
            return new EstimacionResumenDto(
                e.Id, e.NumeroCorrelativo, e.Periodo,
                e.Estado, e.EstadoPago,
                montoEst, e.MontoPagadoAcumulado, montoEst - e.MontoPagadoAcumulado);
        }).ToList();
    }

    public async Task<EstimacionDetalleDto> ObtenerDetalleAsync(int estimacionId, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetConDetalleAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        var montoAcumulado = await _context.Estimaciones
            .Where(e => e.ContratoId == estimacion.ContratoId &&
                        e.Estado == EstadoEstimacion.AprobadaResidencia &&
                        e.Id != estimacionId)
            .Include(e => e.Conceptos)
            .SelectMany(e => e.Conceptos)
            .SumAsync(c => c.Importe, ct);

        var pagos = await _context.EstimacionPagos
            .Where(p => p.EstimacionId == estimacionId)
            .OrderBy(p => p.FechaRegistro)
            .ToListAsync(ct);

        var montoContratado = estimacion.Contrato!.MontoContratado;
        var montoEstimado = estimacion.Conceptos.Sum(c => c.Importe);

        return new EstimacionDetalleDto(
            estimacion.Id, estimacion.ContratoId, estimacion.NumeroCorrelativo, estimacion.Periodo,
            estimacion.Estado, estimacion.EstadoPago,
            montoEstimado, montoContratado, montoAcumulado, montoContratado - montoAcumulado,
            estimacion.MontoPagadoAcumulado,
            estimacion.FechaAprobacionSupervision, estimacion.UsuarioAprobacionSupervisionId,
            estimacion.FechaAprobacionResidencia, estimacion.UsuarioAprobacionResidenciaId,
            estimacion.Conceptos.Select(c => new EstimacionConceptoDetalleDto(
                c.ConceptoContrato!.Clave, c.ConceptoContrato.Descripcion,
                c.CantidadEjecutada, c.ConceptoContrato.PrecioUnitario, c.Importe)).ToList(),
            estimacion.NotasVinculadas.Select(n => $"Folio {n.NotaBitacora!.Folio} — {n.NotaBitacora.Asunto}").ToList(),
            estimacion.Observaciones.Select(o => new ObservacionDto(o.Id, o.Texto, o.FechaCreacion, o.UsuarioId)).ToList(),
            pagos.Select(p => new PagoEstimacionDto(p.Id, p.FechaPago, p.ReferenciaBancaria, p.MontoPagado, p.UsuarioRegistroId, p.FechaRegistro)).ToList()
        );
    }

    public async Task ActualizarConceptosAsync(int estimacionId, ActualizarConceptosEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _context.Estimaciones.Include(e => e.Conceptos).FirstOrDefaultAsync(e => e.Id == estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        _context.EstimacionConceptos.RemoveRange(estimacion.Conceptos);

        foreach (var item in dto.Conceptos)
        {
            var conceptoContrato = await _context.ConceptosContrato.FindAsync(new object[] { item.ConceptoContratoId }, ct)
                ?? throw new InvalidOperationException($"Concepto de catálogo {item.ConceptoContratoId} no encontrado.");

            estimacion.Conceptos.Add(new EstimacionConcepto
            {
                EstimacionId = estimacionId,
                ConceptoContratoId = item.ConceptoContratoId,
                CantidadEjecutada = item.CantidadEjecutada,
                Importe = item.CantidadEjecutada * conceptoContrato.PrecioUnitario
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
                EstimacionId = estimacionId,
                NotaBitacoraId = notaId
            });
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task EnviarAsync(int estimacionId, int usuarioId, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetByIdAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        if (estimacion.Estado != EstadoEstimacion.Borrador && estimacion.Estado != EstadoEstimacion.ObservadaSupervision)
            throw new InvalidOperationException($"Solo se puede enviar una estimación en estado Borrador u ObservadaSupervision. Estado actual: {estimacion.Estado}.");

        var estadoAnterior = estimacion.Estado;
        estimacion.Estado = EstadoEstimacion.Enviada;
        estimacion.FechaEnvio = DateTime.UtcNow;
        estimacion.UsuarioEnvioId = usuarioId;

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = EstadoEstimacion.Enviada,
            Fecha = estimacion.FechaEnvio.Value,
            UsuarioId = usuarioId
        });

        await _context.SaveChangesAsync(ct);

        await _alertaService.EmitirAsync(estimacion.ContratoId, TipoAlerta.EstimacionPlazoRevision, estimacion.Id,
            nameof(Estimacion), RolSistema.SupervisorExterno,
            $"Estimación N.° {estimacion.NumeroCorrelativo} enviada para revisión de supervisión.", ct);
    }

    public async Task<ObservacionDto> AgregarObservacionAsync(int estimacionId, CrearObservacionDto dto, int usuarioId, CancellationToken ct = default)
    {
        var observacion = new EstimacionObservacion
        {
            EstimacionId = estimacionId,
            Texto = dto.Texto,
            UsuarioId = usuarioId
        };

        _context.EstimacionObservaciones.Add(observacion);
        await _context.SaveChangesAsync(ct);

        return new ObservacionDto(observacion.Id, observacion.Texto, observacion.FechaCreacion, observacion.UsuarioId);
    }

    public async Task CambiarEstadoAsync(int estimacionId, CambiarEstadoEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetByIdAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        var (from, to) = (estimacion.Estado, dto.NuevoEstado);

        bool transicionValida = (from, to) switch
        {
            (EstadoEstimacion.Enviada, EstadoEstimacion.ObservadaSupervision) => true,
            (EstadoEstimacion.Enviada, EstadoEstimacion.AprobadaSupervision) => true,
            (EstadoEstimacion.AprobadaSupervision, EstadoEstimacion.AprobadaResidencia) => true,
            (EstadoEstimacion.AprobadaSupervision, EstadoEstimacion.RechazadaResidencia) => true,
            _ => false
        };

        if (!transicionValida)
            throw new InvalidOperationException($"Transición no permitida: {from} → {to}.");

        if (to == EstadoEstimacion.RechazadaResidencia && string.IsNullOrWhiteSpace(dto.Comentario))
            throw new InvalidOperationException("El rechazo requiere un motivo.");

        var estadoAnterior = estimacion.Estado;
        estimacion.Estado = to;

        if (to == EstadoEstimacion.AprobadaSupervision)
        {
            estimacion.FechaAprobacionSupervision = DateTime.UtcNow;
            estimacion.UsuarioAprobacionSupervisionId = dto.UsuarioId;
        }
        else if (to == EstadoEstimacion.AprobadaResidencia)
        {
            estimacion.FechaAprobacionResidencia = DateTime.UtcNow;
            estimacion.UsuarioAprobacionResidenciaId = dto.UsuarioId;
        }

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = to,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.UsuarioId,
            Comentario = dto.Comentario
        });

        await _context.SaveChangesAsync(ct);

        // Resolver alerta anterior y emitir la siguiente según el nuevo estado
        if (to == EstadoEstimacion.ObservadaSupervision)
        {
            // La estimación vuelve al superintendente para corrección; dejar alerta activa
        }
        else if (to == EstadoEstimacion.AprobadaSupervision)
        {
            await _alertaService.ResolverAsync(TipoAlerta.EstimacionPlazoRevision, estimacion.Id, ct);
            await _alertaService.EmitirAsync(estimacion.ContratoId, TipoAlerta.EstimacionPlazoRevision, estimacion.Id,
                nameof(Estimacion), RolSistema.Residencia,
                $"Estimación N.° {estimacion.NumeroCorrelativo} aprobada por supervisión, pendiente revisión de residencia.", ct);
        }
        else if (to == EstadoEstimacion.AprobadaResidencia || to == EstadoEstimacion.RechazadaResidencia)
        {
            await _alertaService.ResolverAsync(TipoAlerta.EstimacionPlazoRevision, estimacion.Id, ct);
        }
    }

    public async Task RegistrarPagoAsync(int estimacionId, RegistrarPagoEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _context.Estimaciones
            .Include(e => e.Pagos)
            .FirstOrDefaultAsync(e => e.Id == estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        if (estimacion.Estado != EstadoEstimacion.AprobadaResidencia)
            throw new InvalidOperationException("Solo se pueden registrar pagos en estimaciones aprobadas por residencia.");

        if (dto.MontoPagado <= 0)
            throw new InvalidOperationException("El monto pagado debe ser mayor a cero.");

        var montoEstimado = await _context.EstimacionConceptos
            .Where(c => c.EstimacionId == estimacionId)
            .SumAsync(c => c.Importe, ct);

        var nuevoAcumulado = estimacion.MontoPagadoAcumulado + dto.MontoPagado;
        if (nuevoAcumulado > montoEstimado + 0.01m)
            throw new InvalidOperationException(
                $"El pago excede el monto estimado ({montoEstimado:N2}). Acumulado actual: {estimacion.MontoPagadoAcumulado:N2}.");

        _context.EstimacionPagos.Add(new EstimacionPago
        {
            EstimacionId = estimacionId,
            FechaPago = dto.FechaPago,
            ReferenciaBancaria = dto.ReferenciaBancaria,
            MontoPagado = dto.MontoPagado,
            UsuarioRegistroId = dto.UsuarioRegistroId,
            FechaRegistro = DateTime.UtcNow
        });

        estimacion.MontoPagadoAcumulado = nuevoAcumulado;
        estimacion.EstadoPago = nuevoAcumulado >= montoEstimado - 0.01m
            ? EstadoPagoEstimacion.Pagada
            : EstadoPagoEstimacion.PagoParcial;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<EstimacionHistorialDto>> ObtenerHistorialAsync(int estimacionId, CancellationToken ct = default)
    {
        var historial = await _context.EstimacionHistoriales
            .Where(h => h.EstimacionId == estimacionId)
            .OrderBy(h => h.Fecha)
            .ToListAsync(ct);

        return historial.Select(h => new EstimacionHistorialDto(
            h.EstadoAnterior, h.EstadoNuevo, h.Fecha, h.UsuarioId, h.Comentario)).ToList();
    }

    public async Task<List<PagoEstimacionDto>> ObtenerPagosAsync(int estimacionId, CancellationToken ct = default)
    {
        var pagos = await _context.EstimacionPagos
            .Where(p => p.EstimacionId == estimacionId)
            .OrderBy(p => p.FechaRegistro)
            .ToListAsync(ct);

        return pagos.Select(p => new PagoEstimacionDto(
            p.Id, p.FechaPago, p.ReferenciaBancaria, p.MontoPagado, p.UsuarioRegistroId, p.FechaRegistro)).ToList();
    }
}
