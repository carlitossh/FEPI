using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class EntregaRecepcionService : IEntregaRecepcionService
{
    private readonly FepiDbContext _context;

    public EntregaRecepcionService(FepiDbContext context) => _context = context;

    public async Task IniciarAsync(IniciarEntregaRecepcionDto dto, CancellationToken ct = default)
    {
        var entrega = new EntregaRecepcion
        {
            Id = int.Newint(), ContratoId = dto.ContratoId, FechaEntrega = dto.FechaEntrega,
            EstadoObraDescripcion = dto.EstadoObraDescripcion, EstadoGarantiasDescripcion = dto.EstadoGarantiasDescripcion
        };

        foreach (var url in dto.UrlsEvidencia)
            entrega.Evidencias.Add(new EntregaRecepcionEvidencia { Id = int.Newint(), UrlArchivo = url });

        _context.EntregasRecepcion.Add(entrega);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<EntregaRecepcionDto> ObtenerAsync(int contratoId, CancellationToken ct = default)
    {
        var e = await _context.EntregasRecepcion.FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct)
            ?? throw new InvalidOperationException("No existe entrega-recepción registrada para este contrato.");

        return new EntregaRecepcionDto(e.Id, e.FechaEntrega, e.EstadoObraDescripcion);
    }

    public async Task<FiniquitoDto> EmitirFiniquitoAsync(int contratoId, EmitirFiniquitoDto dto, CancellationToken ct = default)
    {
        var totalPagado = await _context.EstimacionPagos
            .Where(p => p.Estimacion!.ContratoId == contratoId && p.Estimacion.Estado == EstadoEstimacion.Pagada)
            .SumAsync(p => p.MontoPagado, ct);

        var totalPendiente = await _context.Estimaciones
            .Where(e => e.ContratoId == contratoId && e.Estado == EstadoEstimacion.Aprobada)
            .Include(e => e.Conceptos).SelectMany(e => e.Conceptos).SumAsync(c => c.Importe, ct);

        var finiquito = await _context.Finiquitos.FirstOrDefaultAsync(f => f.ContratoId == contratoId, ct);

        if (finiquito is null)
        {
            finiquito = new Finiquito { Id = int.Newint(), ContratoId = contratoId };
            _context.Finiquitos.Add(finiquito);
        }

        finiquito.TotalPagado = totalPagado;
        finiquito.TotalPendiente = totalPendiente;
        finiquito.TotalDeductivas = dto.TotalDeductivas;
        finiquito.TotalRetenciones = dto.TotalRetenciones;
        finiquito.UrlReporteFiniquito = $"/reportes/finiquito-{contratoId}.pdf";

        await _context.SaveChangesAsync(ct);

        return MapFiniquitoDto(finiquito);
    }

    public async Task<FiniquitoDto> ObtenerFiniquitoAsync(int contratoId, CancellationToken ct = default)
    {
        var f = await _context.Finiquitos.FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct)
            ?? throw new InvalidOperationException("No existe finiquito registrado para este contrato.");
        return MapFiniquitoDto(f);
    }

    private static FiniquitoDto MapFiniquitoDto(Finiquito f) =>
        new(f.Id, f.TotalPagado, f.TotalPendiente, f.TotalDeductivas, f.TotalRetenciones, f.MontoFinal, f.UrlReporteFiniquito);
}
