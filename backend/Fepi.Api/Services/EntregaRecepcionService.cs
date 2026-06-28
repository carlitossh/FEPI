using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class EntregaRecepcionService : IEntregaRecepcionService
{
    private readonly FepiDbContext _context;

    public EntregaRecepcionService(FepiDbContext context)
    {
        _context = context;
    }

    public async Task IniciarAsync(
        IniciarEntregaRecepcionDto dto,
        CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == dto.ContratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var existeEntrega = await _context.EntregasRecepcion
            .AnyAsync(e => e.ContratoId == dto.ContratoId, ct);

        if (existeEntrega)
            throw new InvalidOperationException("Ya existe una entrega-recepción para este contrato.");

        var entrega = new EntregaRecepcion
        {
            ContratoId = dto.ContratoId,
            FechaEntrega = dto.FechaEntrega,
            EstadoObraDescripcion = dto.EstadoObraDescripcion,
            EstadoGarantiasDescripcion = dto.EstadoGarantiasDescripcion
        };

        foreach (var url in dto.UrlsEvidencia ?? new List<string>())
        {
            entrega.Evidencias.Add(new EntregaRecepcionEvidencia
            {
                UrlArchivo = url
            });
        }

        _context.EntregasRecepcion.Add(entrega);

        // Si tu enum no tiene este valor, cambia por el estado correcto.
        // contrato.Estado = EstadoContrato.EnEntregaRecepcion;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<EntregaRecepcionDto?> ObtenerAsync(
        int contratoId,
        CancellationToken ct = default)
    {
        var entrega = await _context.EntregasRecepcion
            .FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct);

        if (entrega is null) return null;

        return new EntregaRecepcionDto(
            entrega.Id,
            entrega.FechaEntrega,
            entrega.EstadoObraDescripcion
        );
    }

    public async Task<FiniquitoDto> EmitirFiniquitoAsync(
        int contratoId,
        EmitirFiniquitoDto dto,
        CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var existeEntrega = await _context.EntregasRecepcion
            .AnyAsync(e => e.ContratoId == contratoId, ct);

        if (!existeEntrega)
            throw new InvalidOperationException("No se puede emitir finiquito sin entrega-recepción.");

        var totalPagado = await _context.EstimacionPagos
            .Where(p =>
                p.Estimacion != null &&
                p.Estimacion.ContratoId == contratoId &&
                p.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia)
            .SumAsync(p => p.MontoPagado, ct);

        var totalPendiente = await _context.EstimacionConceptos
            .Where(c =>
                c.Estimacion != null &&
                c.Estimacion.ContratoId == contratoId &&
                c.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia)
            .SumAsync(c => c.ImporteTotal, ct);

        var finiquito = await _context.Finiquitos
            .FirstOrDefaultAsync(f => f.ContratoId == contratoId, ct);

        if (finiquito is null)
        {
            finiquito = new Finiquito
            {
                ContratoId = contratoId
            };

            _context.Finiquitos.Add(finiquito);
        }

        finiquito.TotalPagado = totalPagado;
        finiquito.TotalPendiente = totalPendiente;
        finiquito.TotalDeductivas = dto.TotalDeductivas;
        finiquito.TotalRetenciones = dto.TotalRetenciones;
        finiquito.UrlReporteFiniquito = $"/reportes/finiquito-{contratoId}.pdf";
        finiquito.FechaEmision = DateTime.UtcNow;

        // Si tu enum tiene Finiquitado, puedes activar esta línea.
        // contrato.Estado = EstadoContrato.Finiquitado;

        await _context.SaveChangesAsync(ct);

        return MapFiniquitoDto(finiquito);
    }

    public async Task<FiniquitoDto?> ObtenerFiniquitoAsync(
        int contratoId,
        CancellationToken ct = default)
    {
        var finiquito = await _context.Finiquitos
            .FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct);

        if (finiquito is null) return null;

        return MapFiniquitoDto(finiquito);
    }

    private static FiniquitoDto MapFiniquitoDto(Finiquito f)
    {
        return new FiniquitoDto(
            f.Id,
            f.TotalPagado,
            f.TotalPendiente,
            f.TotalDeductivas,
            f.TotalRetenciones,
            f.MontoFinal,
            f.UrlReporteFiniquito
        );
    }
}