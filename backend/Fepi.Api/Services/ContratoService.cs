using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class ContratoService : IContratoService
{
    private readonly FepiDbContext _context;

    public ContratoService(FepiDbContext context)
    {
        _context = context;
    }

    public async Task<int> CrearAsync(CrearContratoDto dto, CancellationToken ct = default)
    {
        var existe = await _context.Contratos
            .AnyAsync(c => c.NumeroContrato == dto.NumeroContrato, ct);

        if (existe)
            throw new InvalidOperationException("Ya existe un contrato con ese número.");

        // Calcular periodos
        var nPeriodos = CalcularNumeroPeriodos(dto.FechaInicio, dto.FechaTermino, dto.TipoPeriodo ?? TipoPeriodoEstimacion.Mensual);
        var montoAnticipo = Math.Round(dto.MontoContratado * (dto.PorcentajeAnticipo ?? 0) / 100, 2);
        var importeSinIva = Math.Round(dto.MontoContratado / 1.16m, 2);
        var iva = dto.MontoContratado - importeSinIva;

        var empresaId = dto.EmpresaId ?? 0;
        if (empresaId == 0)
            throw new InvalidOperationException("Se requiere una empresa contratista.");

        var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == empresaId, ct);
        if (!empresaExiste)
            throw new InvalidOperationException("Empresa contratista no encontrada.");

        var contrato = new Contrato
        {
            NumeroContrato = dto.NumeroContrato,
            NombreObra = dto.NombreObra,
            Tipo = dto.Tipo,
            EmpresaId = empresaId,
            ImporteTotal = dto.MontoContratado,
            ImporteSinIVA = importeSinIva,
            IVA = iva,
            ModalidadPago = dto.ModalidadPago ?? ModalidadPago.PrecioUnitario,
            PorcentajeAnticipo = dto.PorcentajeAnticipo ?? 0,
            MontoAnticipo = montoAnticipo,
            FechaInicio = dto.FechaInicio,
            FechaTermino = dto.FechaTermino,
            TipoPeriodo = dto.TipoPeriodo ?? TipoPeriodoEstimacion.Mensual,
            NumeroPeriodos = nPeriodos,
            DependenciaContratante = dto.DependenciaContratante,
            ResidenteNombre = dto.ResidenteNombre,
            SupervisorExternoNombre = dto.SupervisorExternoNombre,
            SuperintendenteNombre = dto.SuperintendenteNombre,
            Estado = EstadoContrato.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        // Generar periodos automáticamente
        GenerarPeriodos(contrato);

        foreach (var g in dto.Garantias ?? [])
        {
            contrato.Garantias.Add(new Garantia
            {
                Tipo = g.Tipo,
                Monto = g.Monto,
                Porcentaje = g.Porcentaje,
                Vigencia = g.Vigencia
            });
        }

        _context.Contratos.Add(contrato);
        await _context.SaveChangesAsync(ct);

        return contrato.Id;
    }

    public async Task<ContratoDetalleDto> ObtenerDetalleAsync(int contratoId, CancellationToken ct = default)
    {
        var c = await _context.Contratos
            .Include(x => x.Empresa)
            .Include(x => x.ConceptoContratos)
            .Include(x => x.Garantias)
            .FirstOrDefaultAsync(x => x.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var empresaNombre = c.Empresa?.Nombre ?? "";

        return new ContratoDetalleDto(
            c.Id,
            c.NumeroContrato,
            c.Tipo,
            c.ImporteTotal,
            c.FechaInicio,
            c.FechaTermino,
            c.DependenciaContratante,
            empresaNombre,
            "",
            c.Estado,
            c.ConceptoContratos.Where(x => x.Activo).Sum(x => x.PrecioUnitario * x.CantidadContratada),
            c.NombreObra,
            c.ResidenteNombre,
            c.SupervisorExternoNombre,
            c.SuperintendenteNombre,
            c.ConceptoContratos
                .Select(x => new ConceptoContratoDto(
                    x.Id, x.Clave, x.Descripcion, x.UnidadMedida,
                    x.CantidadContratada, x.PrecioUnitario,
                    x.PrecioUnitario * x.CantidadContratada))
                .ToList(),
            c.Garantias
                .Select(x => new GarantiaDto(x.Id, x.Tipo, x.Monto, x.Porcentaje, x.Vigencia, x.Estado))
                .ToList()
        );
    }

    public async Task<List<ContratoResumenDto>> ListarPorDependenciaAsync(
        string? dependencia,
        EstadoContrato? estado,
        CancellationToken ct = default)
    {
        var query = _context.Contratos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(dependencia))
            query = query.Where(c => c.DependenciaContratante == dependencia);

        if (estado.HasValue)
            query = query.Where(c => c.Estado == estado.Value);

        var contratos = await query.OrderByDescending(c => c.FechaCreacion).ToListAsync(ct);
        var resultado = new List<ContratoResumenDto>();

        foreach (var contrato in contratos)
        {
            var montoEstimado = await _context.EstimacionConceptos
                .Where(ec => ec.Estimacion != null
                    && ec.Estimacion.ContratoId == contrato.Id
                    && ec.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia)
                .SumAsync(ec => ec.ImporteTotal, ct);

            var montoPagado = await _context.EstimacionPagos
                .Where(p => p.Estimacion != null && p.Estimacion.ContratoId == contrato.Id)
                .SumAsync(p => p.MontoPagado, ct);

            resultado.Add(new ContratoResumenDto(
                contrato.Id, contrato.NumeroContrato, contrato.ImporteTotal,
                montoEstimado, montoPagado, contrato.Estado));
        }

        return resultado;
    }

    public async Task ActualizarProgramaObraAsync(
        int contratoId,
        ActualizarProgramaObraDto dto,
        CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .Include(c => c.ProgramaObra)
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        _context.ProgramaObraItems.RemoveRange(contrato.ProgramaObra);

        foreach (var p in dto.ProgramaObra)
        {
            contrato.ProgramaObra.Add(new ProgramaObraItem
            {
                Periodo = p.Periodo,
                PorcentajeProgramado = p.PorcentajeProgramado,
                MontoProgramado = p.MontoProgramado
            });
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task AgregarOActualizarConceptoCatalogoAsync(
        int contratoId,
        ConceptoContratoInputDto dto,
        CancellationToken ct = default)
    {
        var contratoExiste = await _context.Contratos.AnyAsync(c => c.Id == contratoId, ct);
        if (!contratoExiste)
            throw new InvalidOperationException("Contrato no encontrado.");

        var existente = await _context.ConceptosContrato
            .FirstOrDefaultAsync(c => c.ContratoId == contratoId && c.Clave == dto.Clave, ct);

        if (existente is not null)
        {
            existente.Descripcion = dto.Descripcion;
            existente.UnidadMedida = dto.UnidadMedida;
            existente.CantidadContratada = dto.CantidadContratada;
            existente.PrecioUnitario = dto.PrecioUnitario;
        }
        else
        {
            _context.ConceptosContrato.Add(new ConceptoContrato
            {
                ContratoId = contratoId,
                Clave = dto.Clave,
                Descripcion = dto.Descripcion,
                UnidadMedida = dto.UnidadMedida,
                CantidadContratada = dto.CantidadContratada,
                PrecioUnitario = dto.PrecioUnitario
            });
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task ActualizarMontoContratadoAsync(
        int contratoId,
        decimal nuevoMonto,
        CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        contrato.ImporteTotal = nuevoMonto;
        contrato.ImporteSinIVA = Math.Round(nuevoMonto / 1.16m, 2);
        contrato.IVA = nuevoMonto - contrato.ImporteSinIVA;
        contrato.MontoAnticipo = Math.Round(nuevoMonto * contrato.PorcentajeAnticipo / 100, 2);

        await _context.SaveChangesAsync(ct);
    }

    public async Task ActualizarAsync(int contratoId, ActualizarContratoDto dto, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        if (contrato.NumeroContrato != dto.NumeroContrato)
        {
            var existe = await _context.Contratos
                .AnyAsync(c => c.NumeroContrato == dto.NumeroContrato, ct);
            if (existe)
                throw new InvalidOperationException("Ya existe un contrato con ese número.");
        }

        contrato.NumeroContrato = dto.NumeroContrato;
        contrato.NombreObra = dto.NombreObra;
        contrato.Tipo = dto.Tipo;
        contrato.ImporteTotal = dto.MontoContratado;
        contrato.ImporteSinIVA = Math.Round(dto.MontoContratado / 1.16m, 2);
        contrato.IVA = dto.MontoContratado - contrato.ImporteSinIVA;
        contrato.FechaInicio = dto.FechaInicio;
        contrato.FechaTermino = dto.FechaTermino;
        contrato.DependenciaContratante = dto.DependenciaContratante;
        contrato.ResidenteNombre = dto.ResidenteNombre;
        contrato.SupervisorExternoNombre = dto.SupervisorExternoNombre;
        contrato.SuperintendenteNombre = dto.SuperintendenteNombre;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<ContratoDetalleCompletoDto> ObtenerDetalleCompletoAsync(int contratoId, CancellationToken ct = default)
    {
        var c = await _context.Contratos
            .Include(x => x.Empresa).ThenInclude(e => e!.RepresentanteUsuario)
            .Include(x => x.Periodos)
            .Include(x => x.SeccionesConcepto).ThenInclude(s => s.Conceptos)
            .Include(x => x.ProgramaObraSecciones)
            .FirstOrDefaultAsync(x => x.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        // Estimaciones
        var estimaciones = await _context.Estimaciones
            .Include(e => e.Conceptos)
            .Where(e => e.ContratoId == contratoId)
            .ToListAsync(ct);

        var montoEstimadoTotal = estimaciones
            .Where(e => e.Estado == EstadoEstimacion.AprobadaResidencia)
            .SelectMany(e => e.Conceptos)
            .Sum(ec => ec.ImporteTotal);

        var montoPagadoTotal = estimaciones.Sum(e => e.MontoPagadoAcumulado);

        var resumenEst = new ResumenEstimacionesDetalleDto(
            estimaciones.Count,
            estimaciones.Count(e => e.Estado == EstadoEstimacion.Borrador),
            estimaciones.Count(e => e.Estado == EstadoEstimacion.Enviada),
            estimaciones.Count(e => e.Estado is EstadoEstimacion.AprobadaSupervision or EstadoEstimacion.AprobadaResidencia),
            estimaciones.Count(e => e.EstadoPago == EstadoPagoEstimacion.Pagada),
            montoEstimadoTotal, montoPagadoTotal, montoEstimadoTotal - montoPagadoTotal);

        // Convenios
        var convenios = await _context.ConveniosModificatorios
            .Where(cv => cv.ContratoId == contratoId)
            .ToListAsync(ct);

        var varMonto = convenios
            .Where(cv => cv.Estado == EstadoConvenio.Aplicado)
            .Sum(cv => cv.MontoSolicitado ?? 0);

        var varDias = convenios
            .Where(cv => cv.Estado == EstadoConvenio.Aplicado)
            .Sum(cv => cv.PlazoDiasSolicitado ?? 0);

        var resumenConv = new ResumenConveniosDetalleDto(
            convenios.Count,
            convenios.Count(cv => cv.Estado == EstadoConvenio.Aplicado),
            convenios.Count(cv => cv.Estado != EstadoConvenio.Aplicado),
            varMonto == 0 ? null : varMonto,
            varDias == 0 ? null : varDias);

        // Bitácora
        var bitacora = await _context.Bitacoras
            .Include(b => b.Notas)
            .FirstOrDefaultAsync(b => b.ContratoId == contratoId, ct);

        ResumenBitacoraDetalleDto? resumenBit = null;
        if (bitacora is not null)
        {
            resumenBit = new ResumenBitacoraDetalleDto(
                bitacora.Estado == EstadoBitacora.Abierta,
                bitacora.Notas.Count,
                bitacora.Notas.Count(n => n.EstadoFirma == EstadoFirmaNota.Pendiente));
        }

        // Registros diarios
        var totalRegistros = await _context.RegistrosDiarios
            .CountAsync(r => r.ContratoId == contratoId, ct);

        // Finiquito
        var finiquito = await _context.FiniquitosContrato
            .FirstOrDefaultAsync(f => f.ContratoId == contratoId, ct);

        FiniquitoResumenDetalleDto? resumenFin = null;
        if (finiquito is not null)
        {
            resumenFin = new FiniquitoResumenDetalleDto(
                finiquito.Id,
                finiquito.Estado,
                finiquito.ImporteFinalAFiniquitar == 0 ? null : finiquito.ImporteFinalAFiniquitar);
        }

        // Alertas recientes del contrato (últimas 5)
        var alertas = await _context.AlertasUsuario
            .Where(a => a.ContratoId == contratoId)
            .OrderByDescending(a => a.FechaCreacion)
            .Take(5)
            .Select(a => new AlertaResumenDetalleDto(a.Id, a.Titulo, a.Tipo, a.Prioridad, a.FechaCreacion, a.Leida))
            .ToListAsync(ct);

        // Resumen conceptos
        var todosConceptos = c.SeccionesConcepto.SelectMany(s => s.Conceptos).ToList();
        var importeCatalogo = todosConceptos
            .Where(con => con.Activo)
            .Sum(con => con.CantidadContratada * con.PrecioUnitario);

        var resumenConc = new ResumenConceptosDetalleDto(
            c.SeccionesConcepto.Count,
            todosConceptos.Count,
            todosConceptos.Count(con => con.Activo),
            importeCatalogo);

        var periodos = c.Periodos
            .OrderBy(p => p.Numero)
            .Select(p => new PeriodoContratoResponse(p.Id, p.Numero, p.FechaInicio, p.FechaFin))
            .ToList();

        return new ContratoDetalleCompletoDto(
            c.Id, c.NumeroContrato, c.NumeroLicitacion, c.NombreObra,
            c.Tipo, c.Estado, c.FechaInicio, c.FechaTermino, c.FechaCreacion,
            c.DependenciaContratante, c.UbicacionExacta,
            c.ImporteTotal, c.ImporteSinIVA, c.ModalidadPago,
            c.PorcentajeAnticipo, c.MontoAnticipo, c.NumeroPeriodos,
            c.ResidenteNombre, c.SupervisorExternoNombre, c.SuperintendenteNombre,
            new EmpresaResumenDetalleDto(
                c.Empresa!.Id, c.Empresa.Nombre,
                c.Empresa.RepresentanteUsuario?.Nombre ?? ""),
            periodos,
            resumenConc,
            c.ProgramaObraSecciones.Any(),
            resumenEst,
            resumenConv,
            resumenBit,
            totalRegistros,
            resumenFin,
            alertas);
    }

    private static int CalcularNumeroPeriodos(DateOnly inicio, DateOnly termino, TipoPeriodoEstimacion tipo)
    {
        if (termino <= inicio) return 0;

        double meses = (termino.Year - inicio.Year) * 12 + (termino.Month - inicio.Month)
            + (termino.Day >= inicio.Day ? 0 : -1);

        return tipo == TipoPeriodoEstimacion.Mensual
            ? (int)Math.Ceiling(meses)
            : (int)Math.Ceiling(meses * 2);
    }

    private static void GenerarPeriodos(Contrato contrato)
    {
        var fecha = contrato.FechaInicio;
        var numero = 1;

        while (fecha < contrato.FechaTermino)
        {
            DateOnly fechaFin;
            if (contrato.TipoPeriodo == TipoPeriodoEstimacion.Mensual)
            {
                fechaFin = fecha.AddMonths(1).AddDays(-1);
                if (fechaFin > contrato.FechaTermino) fechaFin = contrato.FechaTermino;
            }
            else
            {
                fechaFin = fecha.Day <= 15
                    ? new DateOnly(fecha.Year, fecha.Month, 15)
                    : fecha.AddMonths(1).AddDays(-fecha.Day);
                if (fechaFin > contrato.FechaTermino) fechaFin = contrato.FechaTermino;
            }

            contrato.Periodos.Add(new PeriodoContrato
            {
                Numero = numero++,
                FechaInicio = fecha,
                FechaFin = fechaFin
            });

            fecha = fechaFin.AddDays(1);
        }

        contrato.NumeroPeriodos = contrato.Periodos.Count;
    }
}
