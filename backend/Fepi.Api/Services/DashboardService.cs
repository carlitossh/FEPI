using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly FepiDbContext _context;
    private readonly IAvanceService _avanceService;

    public DashboardService(FepiDbContext context, IAvanceService avanceService)
    {
        _context = context;
        _avanceService = avanceService;
    }

    public async Task<DashboardContratoDto> ObtenerPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos.FindAsync(new object[] { contratoId }, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var curva = await _avanceService.ObtenerCurvaSAsync(contratoId, ct);
        var avanceProgramado = curva.Puntos.Count > 0 ? curva.Puntos[^1].PorcentajeProgramado : 0;

        var montoEjercido = await _context.Estimaciones
            .Where(e => e.ContratoId == contratoId && (e.Estado == EstadoEstimacion.Aprobada || e.Estado == EstadoEstimacion.Pagada))
            .Include(e => e.Conceptos).SelectMany(e => e.Conceptos).SumAsync(c => c.Importe, ct);

        var estimacionesPendientes = await _context.Estimaciones
            .CountAsync(e => e.ContratoId == contratoId && (e.Estado == EstadoEstimacion.Enviada || e.Estado == EstadoEstimacion.Observada), ct);

        var conveniosActivos = await _context.ConveniosModificatorios
            .CountAsync(c => c.ContratoId == contratoId && c.Estado != EstadoConvenio.Aprobada && c.Estado != EstadoConvenio.Rechazada, ct);

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var garantiasPorVencer = await _context.Garantias
            .CountAsync(g => g.ContratoId == contratoId && g.Estado == EstadoGarantia.Vigente &&
                        (g.Vigencia.DayNumber - hoy.DayNumber) <= 30, ct);

        return new DashboardContratoDto(contratoId, contrato.NumeroContrato, curva.PorcentajeCumplimientoActual,
            avanceProgramado, montoEjercido, contrato.MontoContratado, estimacionesPendientes, conveniosActivos, garantiasPorVencer);
    }
}
