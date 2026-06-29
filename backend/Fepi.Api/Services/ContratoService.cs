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

        // Validar usuario residente
        var residenteExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.ResidenteUsuarioId, ct);
        if (!residenteExiste)
            throw new InvalidOperationException("El usuario residente no existe.");

        var empresaId = dto.EmpresaId ?? 0;
        if (empresaId == 0)
            throw new InvalidOperationException("Se requiere una empresa contratista.");

        var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == empresaId, ct);
        if (!empresaExiste)
            throw new InvalidOperationException("Empresa contratista no encontrada.");

        var ivaPct = dto.IvaPorcentaje <= 0 ? 16m : dto.IvaPorcentaje;
        var iva = Math.Round(dto.ImporteSinIVA * ivaPct / 100m, 2);
        var importeTotal = dto.ImporteSinIVA + iva;
        var nPeriodos = CalcularNumeroPeriodos(dto.FechaInicio, dto.FechaTermino, dto.TipoPeriodo ?? TipoPeriodoEstimacion.Mensual);
        var montoAnticipo = Math.Round(importeTotal * (dto.PorcentajeAnticipo ?? 0) / 100m, 2);

        var contrato = new Contrato
        {
            NumeroContrato = dto.NumeroContrato,
            NombreObra = dto.NombreObra,
            NumeroLicitacion = dto.NumeroLicitacion,
            Tipo = dto.Tipo,
            EmpresaId = empresaId,
            ImporteSinIVA = dto.ImporteSinIVA,
            IvaPorcentaje = ivaPct,
            IVA = iva,
            ImporteTotal = importeTotal,
            ModalidadPago = dto.ModalidadPago ?? ModalidadPago.PrecioUnitario,
            PorcentajeAnticipo = dto.PorcentajeAnticipo ?? 0,
            MontoAnticipo = montoAnticipo,
            FechaInicio = dto.FechaInicio,
            FechaTermino = dto.FechaTermino,
            TipoPeriodo = dto.TipoPeriodo ?? TipoPeriodoEstimacion.Mensual,
            NumeroPeriodos = nPeriodos,
            DependenciaContratante = dto.DependenciaContratante,
            UbicacionExacta = dto.UbicacionExacta,
            ResidenteUsuarioId = dto.ResidenteUsuarioId,
            SupervisorExternoUsuarioId = dto.SupervisorExternoUsuarioId,
            SuperintendenteUsuarioId = dto.SuperintendenteUsuarioId,
            FinancialUsuarioId = dto.FinancialUsuarioId,
            Estado = EstadoContrato.Activo,
            FechaCreacion = DateTime.UtcNow
        };

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
            .Include(x => x.ResidenteUsuario)
            .Include(x => x.SupervisorExternoUsuario)
            .Include(x => x.SuperintendenteUsuario)
            .Include(x => x.FinancialUsuario)
            .Include(x => x.ConceptoContratos)
            .Include(x => x.Garantias)
            .FirstOrDefaultAsync(x => x.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var conceptosActivos = c.ConceptoContratos.Where(x => x.Activo).ToList();
        var importeConceptos = conceptosActivos.Sum(x => x.PrecioUnitario * x.CantidadContratada);
        var diferencia = importeConceptos - c.ImporteSinIVA;
        var cuadra = Math.Abs(diferencia) <= 0.01m;

        var secciones = await _context.SeccionesConcepto.Where(s => s.ContratoId == contratoId).CountAsync(ct);

        return new ContratoDetalleDto(
            c.Id,
            c.NumeroContrato,
            c.Tipo,
            c.ImporteTotal,
            c.ImporteSinIVA,
            c.IvaPorcentaje,
            c.IVA,
            c.FechaInicio,
            c.FechaTermino,
            c.DependenciaContratante,
c.Empresa?.Nombre ?? "",
c.Empresa?.Rfc ?? "",
c.Empresa?.RepresentanteUsuario?.Nombre ?? "",
c.Estado,
            importeConceptos,
            c.NombreObra,
            new ResponsableDto(c.ResidenteUsuarioId, c.ResidenteUsuario?.Nombre),
            c.SupervisorExternoUsuarioId is null ? null : new ResponsableDto(c.SupervisorExternoUsuarioId, c.SupervisorExternoUsuario?.Nombre),
            c.SuperintendenteUsuarioId is null ? null : new ResponsableDto(c.SuperintendenteUsuarioId, c.SuperintendenteUsuario?.Nombre),
            c.FinancialUsuarioId is null ? null : new ResponsableDto(c.FinancialUsuarioId, c.FinancialUsuario?.Nombre),
            c.ConceptoContratos
                .Select(x => new ConceptoContratoDto(
                    x.Id, x.Clave, x.Descripcion, x.UnidadMedida,
                    x.CantidadContratada, x.PrecioUnitario,
                    x.PrecioUnitario * x.CantidadContratada))
                .ToList(),
            c.Garantias
                .Select(x => new GarantiaDto(x.Id, x.Tipo, x.Monto, x.Porcentaje, x.Vigencia, x.Estado))
                .ToList(),
            new ConceptosResumenDto(
                secciones,
                c.ConceptoContratos.Count,
                importeConceptos,
                c.ImporteSinIVA,
                diferencia,
                cuadra)
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
        decimal nuevoImporteSinIVA,
        decimal ivaPorcentaje,
        CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var ivaPct = ivaPorcentaje > 0 ? ivaPorcentaje : contrato.IvaPorcentaje;
        var iva = Math.Round(nuevoImporteSinIVA * ivaPct / 100m, 2);
        contrato.ImporteSinIVA = nuevoImporteSinIVA;
        contrato.IvaPorcentaje = ivaPct;
        contrato.IVA = iva;
        contrato.ImporteTotal = nuevoImporteSinIVA + iva;
        contrato.MontoAnticipo = Math.Round(contrato.ImporteTotal * contrato.PorcentajeAnticipo / 100m, 2);

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

        var residenteExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.ResidenteUsuarioId, ct);
        if (!residenteExiste)
            throw new InvalidOperationException("El usuario residente no existe.");

        var ivaPct = dto.IvaPorcentaje <= 0 ? contrato.IvaPorcentaje : dto.IvaPorcentaje;
        var iva = Math.Round(dto.ImporteSinIVA * ivaPct / 100m, 2);

        contrato.NumeroContrato = dto.NumeroContrato;
        contrato.NombreObra = dto.NombreObra;
        contrato.Tipo = dto.Tipo;
        contrato.ImporteSinIVA = dto.ImporteSinIVA;
        contrato.IvaPorcentaje = ivaPct;
        contrato.IVA = iva;
        contrato.ImporteTotal = dto.ImporteSinIVA + iva;
        contrato.FechaInicio = dto.FechaInicio;
        contrato.FechaTermino = dto.FechaTermino;
        contrato.DependenciaContratante = dto.DependenciaContratante;
        contrato.ResidenteUsuarioId = dto.ResidenteUsuarioId;
        contrato.SupervisorExternoUsuarioId = dto.SupervisorExternoUsuarioId;
        contrato.SuperintendenteUsuarioId = dto.SuperintendenteUsuarioId;
        contrato.FinancialUsuarioId = dto.FinancialUsuarioId;
        contrato.MontoAnticipo = Math.Round(contrato.ImporteTotal * contrato.PorcentajeAnticipo / 100m, 2);

        await _context.SaveChangesAsync(ct);
    }

    public async Task<ContratoDetalleCompletoDto> ObtenerDetalleCompletoAsync(int contratoId, CancellationToken ct = default)
    {
        var c = await _context.Contratos
            .Include(x => x.Empresa).ThenInclude(e => e!.RepresentanteUsuario)
            .Include(x => x.ResidenteUsuario)
            .Include(x => x.SupervisorExternoUsuario)
            .Include(x => x.SuperintendenteUsuario)
            .Include(x => x.FinancialUsuario)
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
            .Where(cv => cv.Aplicado)
            .Sum(cv => cv.MontoSolicitado ?? 0);

        var varDias = convenios
            .Where(cv => cv.Aplicado)
            .Sum(cv => cv.PlazoDiasSolicitado ?? 0);

        var resumenConv = new ResumenConveniosDetalleDto(
            convenios.Count,
            convenios.Count(cv => cv.Aplicado),
            convenios.Count(cv => !cv.Aplicado),
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

        var totalRegistros = await _context.RegistrosDiarios
            .CountAsync(r => r.ContratoId == contratoId, ct);

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

        var alertas = await _context.AlertasUsuario
            .Where(a => a.ContratoId == contratoId)
            .OrderByDescending(a => a.FechaCreacion)
            .Take(5)
            .Select(a => new AlertaResumenDetalleDto(a.Id, a.Titulo, a.Tipo, a.Prioridad, a.FechaCreacion, a.Leida))
            .ToListAsync(ct);

        // Resumen conceptos
        var todosConceptos = c.SeccionesConcepto.SelectMany(s => s.Conceptos).ToList();
        var importeConceptosActivos = todosConceptos
            .Where(con => con.Activo)
            .Sum(con => con.CantidadContratada * con.PrecioUnitario);

        var resumenConc = new ResumenConceptosDetalleDto(
            c.SeccionesConcepto.Count,
            todosConceptos.Count,
            todosConceptos.Count(con => con.Activo),
            importeConceptosActivos);

        var diferencia = importeConceptosActivos - c.ImporteSinIVA;
        var conceptosResumen = new ConceptosResumenDto(
            c.SeccionesConcepto.Count,
            todosConceptos.Count,
            importeConceptosActivos,
            c.ImporteSinIVA,
            diferencia,
            Math.Abs(diferencia) <= 0.01m);

        var periodos = c.Periodos
            .OrderBy(p => p.Numero)
            .Select(p => new PeriodoContratoResponse(p.Id, p.Numero, p.FechaInicio, p.FechaFin))
            .ToList();

        return new ContratoDetalleCompletoDto(
            c.Id, c.NumeroContrato, c.NumeroLicitacion, c.NombreObra,
            c.Tipo, c.Estado, c.FechaInicio, c.FechaTermino, c.FechaCreacion,
            c.DependenciaContratante, c.UbicacionExacta,
            c.ImporteTotal, c.ImporteSinIVA, c.IvaPorcentaje, c.IVA,
            c.ModalidadPago, c.PorcentajeAnticipo, c.MontoAnticipo, c.NumeroPeriodos,
            new ResponsableDto(c.ResidenteUsuarioId, c.ResidenteUsuario?.Nombre),
            c.SupervisorExternoUsuarioId is null ? null : new ResponsableDto(c.SupervisorExternoUsuarioId, c.SupervisorExternoUsuario?.Nombre),
            c.SuperintendenteUsuarioId is null ? null : new ResponsableDto(c.SuperintendenteUsuarioId, c.SuperintendenteUsuario?.Nombre),
            c.FinancialUsuarioId is null ? null : new ResponsableDto(c.FinancialUsuarioId, c.FinancialUsuario?.Nombre),
 new EmpresaResumenDetalleDto(
    c.Empresa!.Id,
    c.Empresa.Nombre,
    c.Empresa.Rfc,
    c.Empresa.RepresentanteUsuario?.Nombre ?? ""),
            periodos,
            resumenConc,
            conceptosResumen,
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
