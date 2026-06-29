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

    private const decimal CincoAlMillarRate = 0.005m;

    public EstimacionService(IEstimacionRepository estimacionRepo, FepiDbContext context, IAlertaService alertaService)
    {
        _estimacionRepo = estimacionRepo;
        _context = context;
        _alertaService = alertaService;
    }

    public async Task<EstimacionResumenDto> CrearAsync(CrearEstimacionDto dto, CancellationToken ct = default)
    {
        var periodo = await _context.PeriodosContrato
            .FirstOrDefaultAsync(p => p.Id == dto.PeriodoContratoId && p.ContratoId == dto.ContratoId, ct)
            ?? throw new InvalidOperationException("Periodo de contrato no encontrado para este contrato.");

        var duplicado = await _context.Estimaciones
            .AnyAsync(e => e.ContratoId == dto.ContratoId && e.PeriodoContratoId == dto.PeriodoContratoId, ct);
        if (duplicado)
            throw new InvalidOperationException($"Ya existe una estimación para el periodo {periodo.FechaInicio:yyyy-MM} en este contrato.");

        var ultimoCorrelativo = await _estimacionRepo.GetUltimoCorrelativoAsync(dto.ContratoId, ct);

        var estimacion = new Estimacion
        {
            ContratoId = dto.ContratoId,
            PeriodoContratoId = dto.PeriodoContratoId,
            NumeroEstimacion = ultimoCorrelativo + 1,
            Periodo = $"{periodo.FechaInicio:yyyy-MM}",
            Estado = EstadoEstimacion.Borrador
        };

        await _estimacionRepo.AddAsync(estimacion, ct);

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacion.Id,
            Accion = "created",
            EstadoNuevo = EstadoEstimacion.Borrador,
            Fecha = DateTime.UtcNow,
            UsuarioId = 0
        });

        await _estimacionRepo.SaveChangesAsync(ct);

        return new EstimacionResumenDto(estimacion.Id, estimacion.NumeroEstimacion, estimacion.Periodo,
            estimacion.Estado, estimacion.EstadoPago, 0, 0, 0);
    }

    public async Task<List<EstimacionResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var estimaciones = await _estimacionRepo.GetByContratoAsync(contratoId, ct);
        return estimaciones.Select(e =>
        {
            var montoEst = e.Conceptos.Sum(c => c.ImporteTotal);
            return new EstimacionResumenDto(
                e.Id, e.NumeroEstimacion, e.Periodo,
                e.Estado, e.EstadoPago,
                montoEst, e.MontoPagadoAcumulado, montoEst - e.MontoPagadoAcumulado);
        }).ToList();
    }

    public async Task<EstimacionDetalleDto> ObtenerDetalleAsync(int estimacionId, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetConDetalleAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        var montoAcumulado = await _context.Estimaciones
            .Where(e => e.ContratoId == estimacion.ContratoId
                && e.Estado == EstadoEstimacion.AprobadaResidencia
                && e.Id != estimacionId)
            .Include(e => e.Conceptos)
            .SelectMany(e => e.Conceptos)
            .SumAsync(c => c.ImporteTotal, ct);

        var pagos = await _context.EstimacionPagos
            .Where(p => p.EstimacionId == estimacionId)
            .OrderBy(p => p.FechaRegistro)
            .ToListAsync(ct);

        var montoContratado = estimacion.Contrato!.ImporteTotal;
        var ivaPct = estimacion.Contrato.IvaPorcentaje;
        var montoSinIva = estimacion.Conceptos.Sum(c => c.ImporteTotal);
        var iva = Math.Round(montoSinIva * ivaPct / 100m, 2);
        var montoConIva = montoSinIva + iva;
        var cincoAlMillar = Math.Round(montoSinIva * CincoAlMillarRate, 2);
        var totalNeto = montoConIva - cincoAlMillar;

        var calculos = new EstimacionCalculosDto(
            montoSinIva, ivaPct, iva, montoConIva,
            0, 0, 0,
            cincoAlMillar, totalNeto);

        return new EstimacionDetalleDto(
            estimacion.Id, estimacion.ContratoId, estimacion.NumeroEstimacion, estimacion.Periodo,
            estimacion.Estado, estimacion.EstadoPago,
            montoSinIva, montoContratado, montoAcumulado, montoContratado - montoAcumulado,
            estimacion.MontoPagadoAcumulado,
            estimacion.FechaAprobacionSupervision, estimacion.UsuarioAprobacionSupervisionId,
            estimacion.FechaAprobacionResidencia, estimacion.UsuarioAprobacionResidenciaId,
            estimacion.Conceptos.Select(c => new EstimacionConceptoDetalleDto(
                c.ConceptoContrato!.Clave, c.ConceptoContrato.Descripcion,
                c.CantidadEjecutadaPeriodo, c.PrecioUnitarioActual, c.ImporteTotal)).ToList(),
            estimacion.NotasVinculadas.Select(n => $"Folio {n.NotaBitacora!.Folio} — {n.NotaBitacora.Asunto}").ToList(),
            estimacion.Observaciones.Select(o => new ObservacionDto(o.Id, o.Texto, o.FechaCreacion, o.UsuarioId)).ToList(),
            pagos.Select(p => new PagoEstimacionDto(p.Id, p.FechaPago, p.ReferenciaBancaria, p.MontoPagado, p.UsuarioRegistroId, p.FechaRegistro)).ToList(),
            calculos
        );
    }

    public async Task ActualizarConceptosAsync(int estimacionId, ActualizarConceptosEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _context.Estimaciones
            .Include(e => e.Conceptos)
            .FirstOrDefaultAsync(e => e.Id == estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        if (estimacion.Estado != EstadoEstimacion.Borrador)
            throw new InvalidOperationException("Solo se pueden editar conceptos en estimaciones en estado Borrador.");

        _context.EstimacionConceptos.RemoveRange(estimacion.Conceptos);

        foreach (var item in dto.Conceptos)
        {
            var conceptoContrato = await _context.ConceptosContrato
                .FindAsync(new object[] { item.ConceptoContratoId }, ct)
                ?? throw new InvalidOperationException($"Concepto {item.ConceptoContratoId} no encontrado.");

            var cantidadAcumuladaAnterior = await _context.EstimacionConceptos
                .Where(ec => ec.ConceptoContratoId == item.ConceptoContratoId
                    && ec.Estimacion!.Estado == EstadoEstimacion.AprobadaResidencia)
                .SumAsync(ec => ec.CantidadEjecutadaPeriodo, ct);

            var cantidadEjecutada = item.CantidadEjecutada;
            var precioUnitario = conceptoContrato.PrecioUnitario;

            if (cantidadAcumuladaAnterior + cantidadEjecutada > conceptoContrato.CantidadContratada)
                throw new InvalidOperationException(
                    $"El concepto {conceptoContrato.Clave} supera la cantidad contratada vigente.");

            estimacion.Conceptos.Add(new EstimacionConcepto
            {
                EstimacionId = estimacionId,
                ConceptoContratoId = item.ConceptoContratoId,
                CantidadEjecutadaPeriodo = cantidadEjecutada,
                PrecioUnitarioActual = precioUnitario,
                ImporteTotal = cantidadEjecutada * precioUnitario,
                CantidadAcumuladaAnterior = cantidadAcumuladaAnterior,
                CantidadAcumuladaActual = cantidadAcumuladaAnterior + cantidadEjecutada,
                CantidadPorEjecutar = conceptoContrato.CantidadContratada - (cantidadAcumuladaAnterior + cantidadEjecutada)
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

        if (estimacion.Estado != EstadoEstimacion.Borrador && estimacion.Estado != EstadoEstimacion.Observada)
            throw new InvalidOperationException($"Solo se puede enviar una estimación en estado Borrador u Observada. Estado actual: {estimacion.Estado}.");

        estimacion.Estado = EstadoEstimacion.Enviada;
        estimacion.FechaEnvio = DateTime.UtcNow;
        estimacion.UsuarioEnvioId = usuarioId;

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId,
            Accion = "submitted",
            EstadoNuevo = EstadoEstimacion.Enviada,
            Fecha = estimacion.FechaEnvio.Value,
            UsuarioId = usuarioId
        });

        await _context.SaveChangesAsync(ct);

        await _alertaService.EmitirAsync(estimacion.ContratoId, TipoAlerta.EstimacionPlazoRevision, estimacion.Id,
            nameof(Estimacion), RolSistema.SupervisorExterno,
            $"Estimación N.° {estimacion.NumeroEstimacion} enviada para revisión de supervisión.", ct);
    }

    public async Task<ObservacionDto> AgregarObservacionAsync(int estimacionId, CrearObservacionDto dto, int usuarioId, CancellationToken ct = default)
    {
        var estimacion = await _estimacionRepo.GetByIdAsync(estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        if (estimacion.Estado == EstadoEstimacion.Enviada)
        {
            estimacion.Estado = EstadoEstimacion.Observada;
            _context.EstimacionHistoriales.Add(new EstimacionHistorial
            {
                EstimacionId = estimacionId,
                Accion = "returned_with_notes",
                EstadoNuevo = EstadoEstimacion.Observada,
                Fecha = DateTime.UtcNow,
                UsuarioId = usuarioId
            });
        }

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
            (EstadoEstimacion.Enviada, EstadoEstimacion.Observada) => true,
            (EstadoEstimacion.Enviada, EstadoEstimacion.AprobadaSupervision) => true,
            (EstadoEstimacion.Observada, EstadoEstimacion.Rechazada) => true,
            (EstadoEstimacion.AprobadaSupervision, EstadoEstimacion.AprobadaResidencia) => true,
            (EstadoEstimacion.AprobadaSupervision, EstadoEstimacion.Rechazada) => true,
            (EstadoEstimacion.AprobadaResidencia, EstadoEstimacion.Pagada) => true,
            _ => false
        };

        if (!transicionValida)
            throw new InvalidOperationException($"Transición no permitida: {from} → {to}.");

        if (to == EstadoEstimacion.Rechazada && string.IsNullOrWhiteSpace(dto.Comentario))
            throw new InvalidOperationException("El rechazo requiere un motivo.");

        estimacion.Estado = to;

        string accion = to switch
        {
            EstadoEstimacion.Observada => "returned_with_notes",
            EstadoEstimacion.AprobadaSupervision => "approved_by_supervision",
            EstadoEstimacion.AprobadaResidencia => "approved_by_residency",
            EstadoEstimacion.Rechazada => "rejected",
            EstadoEstimacion.Pagada => "paid",
            _ => to.ToString().ToLowerInvariant()
        };

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
        else if (to == EstadoEstimacion.Rechazada)
        {
            estimacion.Estado = EstadoEstimacion.Borrador;
        }
        else if (to == EstadoEstimacion.Pagada)
        {
            estimacion.FechaPago = DateTime.UtcNow;
            estimacion.EstadoPago = EstadoPagoEstimacion.Pagada;
        }

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId,
            Accion = accion,
            EstadoNuevo = estimacion.Estado,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.UsuarioId,
            Comentario = dto.Comentario
        });

        await _context.SaveChangesAsync(ct);

        if (to == EstadoEstimacion.AprobadaSupervision)
        {
            await _alertaService.ResolverAsync(TipoAlerta.EstimacionPlazoRevision, estimacion.Id, ct);
            await _alertaService.EmitirAsync(estimacion.ContratoId, TipoAlerta.EstimacionPlazoRevision, estimacion.Id,
                nameof(Estimacion), RolSistema.Residencia,
                $"Estimación N.° {estimacion.NumeroEstimacion} aprobada por supervisión, pendiente revisión de residencia.", ct);
        }
        else if (to == EstadoEstimacion.AprobadaResidencia || to == EstadoEstimacion.Rechazada)
        {
            await _alertaService.ResolverAsync(TipoAlerta.EstimacionPlazoRevision, estimacion.Id, ct);
        }
    }

    public async Task RegistrarPagoAsync(int estimacionId, RegistrarPagoEstimacionDto dto, CancellationToken ct = default)
    {
        var estimacion = await _context.Estimaciones
            .Include(e => e.Pagos)
            .Include(e => e.Conceptos)
            .Include(e => e.Contrato)
            .FirstOrDefaultAsync(e => e.Id == estimacionId, ct)
            ?? throw new InvalidOperationException("Estimación no encontrada.");

        if (estimacion.Estado != EstadoEstimacion.AprobadaResidencia)
            throw new InvalidOperationException("Solo se puede registrar pago en estimaciones aprobadas por residencia.");

        if (estimacion.Pagos.Any())
            throw new InvalidOperationException("Esta estimación ya tiene un pago registrado. No se permiten pagos parciales.");

        // Calcular monto total neto (sin IVA base para 5 al millar, luego aplicar IVA y descuentos)
        var ivaPct = estimacion.Contrato?.IvaPorcentaje ?? 16m;
        var montoSinIva = estimacion.Conceptos.Sum(c => c.ImporteTotal);
        var iva = Math.Round(montoSinIva * ivaPct / 100m, 2);
        var montoConIva = montoSinIva + iva;
        var cincoAlMillar = Math.Round(montoSinIva * CincoAlMillarRate, 2);
        var totalNeto = montoConIva - cincoAlMillar;

        _context.EstimacionPagos.Add(new EstimacionPago
        {
            EstimacionId = estimacionId,
            FechaPago = dto.FechaPago,
            ReferenciaBancaria = dto.ReferenciaBancaria,
            MontoPagado = totalNeto,
            UsuarioRegistroId = dto.UsuarioRegistroId,
            FechaRegistro = DateTime.UtcNow
        });

        estimacion.MontoPagadoAcumulado = totalNeto;
        estimacion.FechaPago = DateTime.UtcNow;
        estimacion.EstadoPago = EstadoPagoEstimacion.Pagada;
        estimacion.Estado = EstadoEstimacion.Pagada;

        if (dto.ArchivoComprobantePagoId.HasValue)
            estimacion.ArchivoComprobantePagoId = dto.ArchivoComprobantePagoId;

        _context.EstimacionHistoriales.Add(new EstimacionHistorial
        {
            EstimacionId = estimacionId,
            Accion = "paid",
            EstadoNuevo = EstadoEstimacion.Pagada,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.UsuarioRegistroId ?? 0
        });

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<EstimacionHistorialDto>> ObtenerHistorialAsync(int estimacionId, CancellationToken ct = default)
    {
        var historial = await _context.EstimacionHistoriales
            .Where(h => h.EstimacionId == estimacionId)
            .OrderBy(h => h.Fecha)
            .ToListAsync(ct);

        return historial.Select(h => new EstimacionHistorialDto(
            h.Id, h.Accion, h.EstadoNuevo, h.Fecha, h.UsuarioId, h.Comentario)).ToList();
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
