using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class AvanceService : IAvanceService
{
    private readonly FepiDbContext _context;

    public AvanceService(FepiDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarAvanceDiarioAsync(
        RegistrarAvanceDto dto,
        CancellationToken ct = default)
    {
        var contratoExiste = await _context.Contratos
            .AnyAsync(c => c.Id == dto.ContratoId, ct);

        if (!contratoExiste)
            throw new InvalidOperationException("Contrato no encontrado.");

        var conceptoExiste = await _context.ConceptosContrato
            .AnyAsync(c => c.Id == dto.ConceptoContratoId && c.ContratoId == dto.ContratoId, ct);

        if (!conceptoExiste)
            throw new InvalidOperationException("Concepto no encontrado para este contrato.");

        var actorExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == dto.ActorId, ct);

        if (!actorExiste)
            throw new InvalidOperationException("Usuario actor no encontrado.");

        var avance = new AvanceDiario
        {
            ContratoId = dto.ContratoId,
            Fecha = dto.Fecha,
            ConceptoContratoId = dto.ConceptoContratoId,
            CantidadEjecutada = dto.CantidadEjecutada,
            DescripcionActividad = dto.DescripcionActividad,
            ActorId = dto.ActorId,
            FechaRegistro = DateTime.UtcNow
        };

        foreach (var url in dto.UrlsEvidencia ?? [])
        {
            avance.Evidencias.Add(new AvanceEvidencia
            {
                UrlFoto = url
            });
        }

        _context.AvancesDiarios.Add(avance);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<CurvaSDto> ObtenerCurvaSAsync(
        int contratoId,
        CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var programa = await _context.ProgramaObraItems
            .Where(p => p.ContratoId == contratoId)
            .OrderBy(p => p.Periodo)
            .ToListAsync(ct);

        var avances = await _context.AvancesDiarios
            .Include(a => a.ConceptoContrato)
            .Where(a => a.ContratoId == contratoId)
            .ToListAsync(ct);

        decimal acumuladoProgramado = 0;
        decimal acumuladoReal = 0;
        var puntos = new List<CurvaSPuntoDto>();

        foreach (var p in programa)
        {
            acumuladoProgramado += p.PorcentajeProgramado;

            var importeRealPeriodo = avances
                .Where(a => a.Fecha.ToString("yyyy-MM") == p.Periodo)
                .Sum(a => a.CantidadEjecutada * (a.ConceptoContrato?.PrecioUnitario ?? 0));

            var pctRealPeriodo = contrato.ImporteTotal > 0
                ? (importeRealPeriodo / contrato.ImporteTotal) * 100
                : 0;

            acumuladoReal += pctRealPeriodo;

            puntos.Add(new CurvaSPuntoDto(
                p.Periodo,
                Math.Round(acumuladoProgramado, 2),
                Math.Round(acumuladoReal, 2)
            ));
        }

        var cumplimiento = puntos.Count > 0
            ? puntos[^1].PorcentajeReal
            : 0;

        return new CurvaSDto(puntos, cumplimiento);
    }

    public async Task<List<AvanceResumenContratoDto>> ObtenerResumenPorDependenciaAsync(
        string dependencia,
        CancellationToken ct = default)
    {
        var contratos = await _context.Contratos
            .Where(c => c.DependenciaContratante == dependencia && c.Estado == EstadoContrato.Activo)
            .ToListAsync(ct);

        var resultado = new List<AvanceResumenContratoDto>();

        foreach (var contrato in contratos)
        {
            var curva = await ObtenerCurvaSAsync(contrato.Id, ct);
            var avanceFisico = curva.PorcentajeCumplimientoActual;

            var montoEjercido = await _context.Estimaciones
                .Where(e =>
                    e.ContratoId == contrato.Id &&
                    e.Estado == EstadoEstimacion.AprobadaResidencia)
                .Include(e => e.Conceptos)
                .SelectMany(e => e.Conceptos)
                .SumAsync(x => x.ImporteTotal, ct);

            var avanceFinanciero = contrato.ImporteTotal > 0
                ? (montoEjercido / contrato.ImporteTotal) * 100
                : 0;

            resultado.Add(new AvanceResumenContratoDto(
                contrato.Id,
                contrato.NumeroContrato,
                Math.Round(avanceFisico, 2),
                Math.Round(avanceFinanciero, 2),
                Math.Round(Math.Abs(avanceFinanciero - avanceFisico), 2)
            ));
        }

        return resultado
            .OrderByDescending(r => r.DesviacionPct)
            .ToList();
    }
}