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
        var contrato = await _context.Contratos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var curva = await _avanceService.ObtenerCurvaSAsync(contratoId, ct);
        var avanceProgramado = curva.Puntos.Count > 0 ? curva.Puntos[^1].PorcentajeProgramado : 0;

        var montoEjercido = await _context.EstimacionConceptos
            .Where(c =>
                c.Estimacion != null &&
                c.Estimacion.ContratoId == contratoId &&
                c.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia)
            .SumAsync(c => c.ImporteTotal, ct);

        var montoPagadoTotal = await _context.EstimacionPagos
            .Where(p => p.Estimacion != null && p.Estimacion.ContratoId == contratoId)
            .SumAsync(p => p.MontoPagado, ct);

        var estimacionesPendientes = await _context.Estimaciones
            .CountAsync(e => e.ContratoId == contratoId &&
                (e.Estado == EstadoEstimacion.Enviada ||
                 e.Estado == EstadoEstimacion.Observada ||
                 e.Estado == EstadoEstimacion.AprobadaSupervision), ct);

        var conveniosActivos = await _context.ConveniosModificatorios
            .CountAsync(c => c.ContratoId == contratoId && c.Estado != EstadoConvenio.Aplicado, ct);

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var garantiasPorVencer = await _context.Garantias
            .CountAsync(g => g.ContratoId == contratoId &&
                g.Estado == EstadoGarantia.Vigente &&
                (g.Vigencia.DayNumber - hoy.DayNumber) >= 0 &&
                (g.Vigencia.DayNumber - hoy.DayNumber) <= 30, ct);

        return new DashboardContratoDto(
            contratoId, contrato.NumeroContrato,
            curva.PorcentajeCumplimientoActual, avanceProgramado,
            montoEjercido, contrato.ImporteTotal,
            estimacionesPendientes, conveniosActivos, garantiasPorVencer,
            montoPagadoTotal);
    }

    public async Task<CurvaFinancieraDto> ObtenerCurvaFinancieraAsync(int contratoId, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var importeTotal = contrato.ImporteTotal;
        if (importeTotal == 0)
            return new CurvaFinancieraDto([], 0, 0);

        var estimadoPorPeriodo = await _context.EstimacionConceptos
            .Where(c => c.Estimacion != null &&
                        c.Estimacion.ContratoId == contratoId &&
                        c.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia)
            .GroupBy(c => c.Estimacion!.Periodo)
            .Select(g => new { Periodo = g.Key, Monto = g.Sum(c => c.ImporteTotal) })
            .ToListAsync(ct);

        var pagosBrutos = await _context.EstimacionPagos
            .Where(p => p.Estimacion != null && p.Estimacion.ContratoId == contratoId)
            .Select(p => new { p.FechaPago, p.MontoPagado })
            .ToListAsync(ct);

        var pagadoPorPeriodo = pagosBrutos
            .GroupBy(p => $"{p.FechaPago.Year:D4}-{p.FechaPago.Month:D2}")
            .ToDictionary(g => g.Key, g => g.Sum(p => p.MontoPagado));

        var periodos = estimadoPorPeriodo.Select(e => e.Periodo)
            .Concat(pagadoPorPeriodo.Keys)
            .Distinct()
            .Order()
            .ToList();

        if (periodos.Count == 0)
            return new CurvaFinancieraDto([], 0, 0);

        decimal cumEst = 0, cumPag = 0;
        var puntos = new List<CurvaFinancieraPuntoDto>();

        foreach (var periodo in periodos)
        {
            cumEst += estimadoPorPeriodo.FirstOrDefault(e => e.Periodo == periodo)?.Monto ?? 0;
            cumPag += pagadoPorPeriodo.GetValueOrDefault(periodo, 0);
            puntos.Add(new CurvaFinancieraPuntoDto(
                periodo,
                Math.Round(cumEst / importeTotal * 100, 2),
                Math.Round(cumPag / importeTotal * 100, 2)
            ));
        }

        return new CurvaFinancieraDto(
            puntos,
            puntos[^1].PorcentajeEstimado,
            puntos[^1].PorcentajePagado
        );
    }

    public async Task<List<VencimientoDto>> ObtenerVencimientosAsync(int contratoId, CancellationToken ct = default)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        var resultado = new List<VencimientoDto>();

        var garantias = await _context.Garantias
            .Where(g => g.ContratoId == contratoId && g.Estado == EstadoGarantia.Vigente)
            .ToListAsync(ct);

        foreach (var g in garantias)
        {
            var dias = g.Vigencia.DayNumber - hoy.DayNumber;
            if (dias <= 60)
            {
                var sev = dias <= 0 ? "critica" : dias <= 15 ? "critica" : dias <= 30 ? "media" : "baja";
                resultado.Add(new VencimientoDto(
                    "Garantía",
                    $"Garantía {g.Tipo} vence el {g.Vigencia:d}",
                    g.Vigencia,
                    dias,
                    sev
                ));
            }
        }

        var bitacora = await _context.Bitacoras.FirstOrDefaultAsync(b => b.ContratoId == contratoId, ct);
        if (bitacora != null)
        {
            var notasPendientes = await _context.BitacoraNotas
                .Include(n => n.Firmas)
                .Where(n => n.BitacoraId == bitacora.Id && !n.Cerrada && n.Firmas.Any(f => !f.Firmado))
                .OrderBy(n => n.FechaRegistro)
                .Take(5)
                .ToListAsync(ct);

            foreach (var n in notasPendientes)
            {
                var limite = DateOnly.FromDateTime(n.FechaRegistro.AddDays(5));
                var dias = limite.DayNumber - hoy.DayNumber;
                resultado.Add(new VencimientoDto(
                    "Bitácora",
                    $"Nota {n.Folio:D4}: {n.Asunto} — pendiente de firma",
                    limite,
                    dias,
                    dias <= 0 ? "critica" : "media"
                ));
            }
        }

        var conveniosPendientes = await _context.ConveniosModificatorios
            .Where(c => c.ContratoId == contratoId && c.Estado == EstadoConvenio.AprobadoResidencia)
            .OrderBy(c => c.FechaEmision)
            .Take(5)
            .ToListAsync(ct);

        foreach (var c in conveniosPendientes)
        {
            var limite = DateOnly.FromDateTime(c.FechaEmision.AddDays(15));
            var dias = limite.DayNumber - hoy.DayNumber;
            resultado.Add(new VencimientoDto(
                "Convenio",
                $"Convenio #{c.Id} pendiente de resolución",
                limite,
                dias,
                dias <= 0 ? "critica" : dias <= 5 ? "media" : "baja"
            ));
        }

        return resultado.OrderBy(v => v.DiasRestantes).ToList();
    }

    public async Task<BitacoraResumenDto> ObtenerBitacoraResumenAsync(int contratoId, CancellationToken ct = default)
    {
        var bitacora = await _context.Bitacoras
            .Include(b => b.Caratula)
            .FirstOrDefaultAsync(b => b.ContratoId == contratoId, ct);

        if (bitacora is null)
            return new BitacoraResumenDto(false, 0, 0, 0, 0, 0, 0, null, null);

        var notas = await _context.BitacoraNotas
            .Include(n => n.Firmas)
            .Where(n => n.BitacoraId == bitacora.Id)
            .ToListAsync(ct);

        var minutas = await _context.BitacoraMinutas.CountAsync(m => m.BitacoraId == bitacora.Id, ct);

        var incidencias = await _context.BitacoraIncidencias
            .Where(i => i.BitacoraId == bitacora.Id)
            .ToListAsync(ct);

        var ultimaNota = notas.OrderByDescending(n => n.FechaRegistro).FirstOrDefault();

        return new BitacoraResumenDto(
            bitacora.Caratula is not null,
            notas.Count,
            notas.Count(n => n.Cerrada),
            notas.Count(n => !n.Cerrada && n.Firmas.Any(f => !f.Firmado)),
            minutas,
            incidencias.Count,
            incidencias.Count(i => i.NotaGeneradaId != null),
            ultimaNota?.Asunto,
            ultimaNota?.FechaRegistro
        );
    }

    public async Task<EstimacionesResumenDto> ObtenerEstimacionesResumenAsync(int contratoId, CancellationToken ct = default)
    {
        var estimaciones = await _context.Estimaciones
            .Where(e => e.ContratoId == contratoId)
            .ToListAsync(ct);

        return new EstimacionesResumenDto(
            estimaciones.Count,
            estimaciones.Count(e => e.Estado == EstadoEstimacion.Borrador),
            estimaciones.Count(e => e.Estado == EstadoEstimacion.Enviada),
            estimaciones.Count(e => e.Estado == EstadoEstimacion.Observada),
            estimaciones.Count(e => e.Estado == EstadoEstimacion.AprobadaSupervision),
            estimaciones.Count(e => e.Estado == EstadoEstimacion.Rechazada),
            estimaciones.Count(e => e.Estado == EstadoEstimacion.AprobadaResidencia),
            estimaciones.Count(e => e.Estado == EstadoEstimacion.Pagada),
            estimaciones.Count(e => e.EstadoPago == EstadoPagoEstimacion.SinPago),
            estimaciones.Count(e => e.EstadoPago == EstadoPagoEstimacion.PagoParcial),
            estimaciones.Count(e => e.EstadoPago == EstadoPagoEstimacion.Pagada)
        );
    }

    public async Task<ConveniosResumenDto> ObtenerConveniosResumenAsync(int contratoId, CancellationToken ct = default)
    {
        var convenios = await _context.ConveniosModificatorios
            .Where(c => c.ContratoId == contratoId)
            .ToListAsync(ct);

        var aplicados = convenios.Where(c => c.Estado == EstadoConvenio.Aplicado).ToList();

        return new ConveniosResumenDto(
            convenios.Count,
            convenios.Count(c => c.Estado != EstadoConvenio.Aplicado),
            aplicados.Count,
            0,
            convenios.Count(c => c.Estado == EstadoConvenio.Revisado || c.Estado == EstadoConvenio.AprobadoSupervision),
            aplicados.Sum(c => c.MontoSolicitado ?? 0),
            aplicados.Sum(c => c.PlazoDiasSolicitado ?? 0)
        );
    }

    public async Task<List<ActividadRecienteDto>> ObtenerActividadRecienteAsync(int contratoId, CancellationToken ct = default)
    {
        var resultado = new List<ActividadRecienteDto>();

        var bitacora = await _context.Bitacoras.FirstOrDefaultAsync(b => b.ContratoId == contratoId, ct);
        if (bitacora != null)
        {
            var notas = await _context.BitacoraNotas
                .Where(n => n.BitacoraId == bitacora.Id)
                .OrderByDescending(n => n.FechaRegistro)
                .Take(8)
                .ToListAsync(ct);

            resultado.AddRange(notas.Select(n => new ActividadRecienteDto(
                n.FechaRegistro, "Bitácora",
                $"Nota {n.Folio:D4}: {n.Asunto}",
                null, $"Folio {n.Folio:D4}"
            )));

            var minutas = await _context.BitacoraMinutas
                .Where(m => m.BitacoraId == bitacora.Id)
                .OrderByDescending(m => m.Fecha)
                .Take(4)
                .ToListAsync(ct);

            resultado.AddRange(minutas.Select(m => new ActividadRecienteDto(
                m.Fecha.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                "Bitácora", "Minuta de reunión registrada", null, null
            )));

            var incidencias = await _context.BitacoraIncidencias
                .Where(i => i.BitacoraId == bitacora.Id)
                .OrderByDescending(i => i.FechaEvento)
                .Take(4)
                .ToListAsync(ct);

            resultado.AddRange(incidencias.Select(i => new ActividadRecienteDto(
                i.FechaEvento.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                "Incidencias",
                i.Descripcion.Length > 80 ? i.Descripcion[..80] + "…" : i.Descripcion,
                null, null
            )));
        }

        var estimaciones = await _context.Estimaciones
            .Include(e => e.UsuarioEnvio)
            .Where(e => e.ContratoId == contratoId)
            .OrderByDescending(e => e.FechaCreacion)
            .Take(10)
            .ToListAsync(ct);

        foreach (var e in estimaciones)
        {
            resultado.Add(new ActividadRecienteDto(
                e.FechaCreacion, "Estimaciones",
                $"Est. #{e.NumeroEstimacion} ({e.Periodo}) — {EstadoLabel(e.Estado)}",
                e.UsuarioEnvio?.Nombre, $"Est. {e.NumeroEstimacion}"
            ));

            if (e.FechaAprobacionResidencia.HasValue)
                resultado.Add(new ActividadRecienteDto(
                    e.FechaAprobacionResidencia.Value, "Estimaciones",
                    $"Est. #{e.NumeroEstimacion} aprobada por residencia",
                    null, $"Est. {e.NumeroEstimacion}"
                ));
        }

        var convenios = await _context.ConveniosModificatorios
            .Include(c => c.Solicitante)
            .Where(c => c.ContratoId == contratoId)
            .OrderByDescending(c => c.FechaEmision)
            .Take(5)
            .ToListAsync(ct);

        resultado.AddRange(convenios.Select(c => new ActividadRecienteDto(
            c.FechaEmision, "Convenios",
            $"Convenio #{c.Id} ({c.Tipo}) — {EstadoConvenioLabel(c.Estado)}",
            c.Solicitante?.Nombre, $"Conv. {c.Id}"
        )));

        return resultado.OrderByDescending(a => a.Fecha).Take(30).ToList();
    }

    private static string EstadoLabel(EstadoEstimacion e) => e switch
    {
        EstadoEstimacion.Borrador             => "Borrador",
        EstadoEstimacion.Enviada              => "Enviada",
        EstadoEstimacion.Observada            => "Observada",
        EstadoEstimacion.AprobadaSupervision  => "Aprobada por supervisión",
        EstadoEstimacion.Rechazada            => "Rechazada",
        EstadoEstimacion.AprobadaResidencia   => "Aprobada por residencia",
        EstadoEstimacion.Pagada               => "Pagada",
        _                                     => e.ToString()
    };

    private static string EstadoConvenioLabel(EstadoConvenio e) => e switch
    {
        EstadoConvenio.Revisado            => "En revisión",
        EstadoConvenio.AprobadoSupervision => "Aprobado por supervisión",
        EstadoConvenio.AprobadoResidencia  => "Aprobado por residencia",
        EstadoConvenio.Aplicado            => "Aplicado",
        _                                  => e.ToString()
    };
}
