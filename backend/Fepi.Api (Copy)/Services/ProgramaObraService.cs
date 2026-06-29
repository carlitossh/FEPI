using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class ProgramaObraService : IProgramaObraService
{
    private readonly FepiDbContext _context;

    public ProgramaObraService(FepiDbContext context)
    {
        _context = context;
    }

    public async Task<int> CrearSeccionProgramaAsync(int contratoId, CrearProgramaSeccionRequest dto, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .Include(c => c.SeccionesConcepto)
                .ThenInclude(s => s.Conceptos)
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var seccion = contrato.SeccionesConcepto.FirstOrDefault(s => s.Id == dto.SeccionConceptoId)
            ?? throw new InvalidOperationException("Sección no encontrada en este contrato.");

        var periodoInicio = await _context.PeriodosContrato
            .FirstOrDefaultAsync(p => p.Id == dto.PeriodoInicioId && p.ContratoId == contratoId, ct)
            ?? throw new InvalidOperationException("Periodo de inicio no encontrado.");

        var periodoFin = await _context.PeriodosContrato
            .FirstOrDefaultAsync(p => p.Id == dto.PeriodoFinId && p.ContratoId == contratoId, ct)
            ?? throw new InvalidOperationException("Periodo de fin no encontrado.");

        if (periodoInicio.Numero > periodoFin.Numero)
            throw new InvalidOperationException("El periodo de inicio debe ser anterior o igual al periodo de fin.");

        var importeSeccion = seccion.Conceptos.Where(c => c.Activo)
            .Sum(c => c.CantidadContratada * c.PrecioUnitario);

        var pesoSeccion = contrato.ImporteTotal > 0 ? importeSeccion / contrato.ImporteTotal : 0;

        // Obtener periodos en el rango
        var periodosEnRango = await _context.PeriodosContrato
            .Where(p => p.ContratoId == contratoId
                && p.Numero >= periodoInicio.Numero
                && p.Numero <= periodoFin.Numero)
            .OrderBy(p => p.Numero)
            .ToListAsync(ct);

        int cantidadPeriodos = periodosEnRango.Count;
        if (cantidadPeriodos == 0)
            throw new InvalidOperationException("No se encontraron periodos en el rango indicado.");

        decimal importePorPeriodo = Math.Round(importeSeccion / cantidadPeriodos, 2);
        decimal avanceParcial = cantidadPeriodos > 0 ? Math.Round(pesoSeccion / cantidadPeriodos, 6) : 0;
        decimal porcentajePeriodo = cantidadPeriodos > 0 ? Math.Round(100m / cantidadPeriodos, 4) : 0;

        var programaSeccion = new ProgramaObraSeccion
        {
            ContratoId = contratoId,
            SeccionConceptoId = dto.SeccionConceptoId,
            PeriodoInicioId = dto.PeriodoInicioId,
            PeriodoFinId = dto.PeriodoFinId
        };

        foreach (var periodo in periodosEnRango)
        {
            programaSeccion.Periodos.Add(new ProgramaObraPeriodo
            {
                PeriodoContratoId = periodo.Id,
                PorcentajePlanificadoPeriodo = porcentajePeriodo,
                ImportePlanificadoPeriodo = importePorPeriodo,
                AvanceParcialPlanificado = avanceParcial
            });
        }

        _context.ProgramaObraSecciones.Add(programaSeccion);
        await _context.SaveChangesAsync(ct);
        return programaSeccion.Id;
    }

    public async Task<ProgramaObraResponse> ObtenerProgramaAsync(int contratoId, CancellationToken ct = default)
    {
        var secciones = await _context.ProgramaObraSecciones
            .Where(s => s.ContratoId == contratoId)
            .Include(s => s.SeccionConcepto)
            .Include(s => s.Periodos)
                .ThenInclude(p => p.PeriodoContrato)
            .ToListAsync(ct);

        var seccionesResponse = secciones.Select(s => new ProgramaObraSeccionResponse(
            s.Id,
            s.SeccionConceptoId,
            s.SeccionConcepto?.Nombre ?? "",
            s.PeriodoInicioId,
            s.PeriodoFinId,
            s.Periodos.OrderBy(p => p.PeriodoContrato!.Numero).Select(p => new ProgramaObraPeriodoResponse(
                p.PeriodoContratoId,
                p.PeriodoContrato!.Numero,
                p.PeriodoContrato.FechaInicio,
                p.PeriodoContrato.FechaFin,
                p.PorcentajePlanificadoPeriodo,
                p.ImportePlanificadoPeriodo,
                p.AvanceParcialPlanificado
            )).ToList()
        )).ToList();

        return new ProgramaObraResponse(contratoId, seccionesResponse);
    }

    public async Task<List<AvancePlanificadoPeriodoDto>> ObtenerAvancePlanificadoAsync(int contratoId, CancellationToken ct = default)
    {
        var periodos = await _context.PeriodosContrato
            .Where(p => p.ContratoId == contratoId)
            .OrderBy(p => p.Numero)
            .ToListAsync(ct);

        var periodosDtos = new List<AvancePlanificadoPeriodoDto>();
        decimal acumuladoAvance = 0;
        decimal acumuladoImporte = 0;

        foreach (var periodo in periodos)
        {
            var avancePeriodo = await _context.ProgramaObraPeriodos
                .Where(pp => pp.PeriodoContratoId == periodo.Id)
                .SumAsync(pp => pp.AvanceParcialPlanificado, ct);

            var importePeriodo = await _context.ProgramaObraPeriodos
                .Where(pp => pp.PeriodoContratoId == periodo.Id)
                .SumAsync(pp => pp.ImportePlanificadoPeriodo, ct);

            acumuladoAvance += avancePeriodo;
            acumuladoImporte += importePeriodo;

            periodosDtos.Add(new AvancePlanificadoPeriodoDto(
                periodo.Id, periodo.Numero, periodo.FechaInicio, periodo.FechaFin,
                Math.Round(acumuladoAvance, 6), Math.Round(acumuladoImporte, 2)));
        }

        return periodosDtos;
    }
}
