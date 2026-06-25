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

        var contrato = new Contrato
        {
            NumeroContrato = dto.NumeroContrato,
            NombreObra = dto.NombreObra,
            Tipo = dto.Tipo,
            MontoContratado = dto.MontoContratado,
            FechaInicio = dto.FechaInicio,
            FechaTermino = dto.FechaTermino,
            PeriodoEstimacion = dto.PeriodoEstimacion,
            DependenciaContratante = dto.DependenciaContratante,
            ContratistaEmpresa = dto.ContratistaEmpresa,
            ContratistaRepresentante = dto.ContratistaRepresentante,
            ResidenteNombre = dto.ResidenteNombre,
            SupervisorExternoNombre = dto.SupervisorExternoNombre,
            SuperintendenteNombre = dto.SuperintendenteNombre,
            Estado = EstadoContrato.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        foreach (var c in dto.ConceptoContratos ?? [])
        {
            contrato.ConceptoContratos.Add(new ConceptoContrato
            {
                Clave = c.Clave,
                Descripcion = c.Descripcion,
                UnidadMedida = c.UnidadMedida,
                CantidadContratada = c.CantidadContratada,
                PrecioUnitario = c.PrecioUnitario,
                Importe = c.CantidadContratada * c.PrecioUnitario
            });
        }

        foreach (var p in dto.ProgramaObra ?? [])
        {
            contrato.ProgramaObra.Add(new ProgramaObraItem
            {
                Periodo = p.Periodo,
                PorcentajeProgramado = p.PorcentajeProgramado,
                MontoProgramado = p.MontoProgramado
            });
        }

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
            .Include(x => x.ConceptoContratos)
            .Include(x => x.Garantias)
            .FirstOrDefaultAsync(x => x.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        return new ContratoDetalleDto(
            c.Id,
            c.NumeroContrato,
            c.Tipo,
            c.MontoContratado,
            c.FechaInicio,
            c.FechaTermino,
            c.DependenciaContratante,
            c.ContratistaEmpresa,
            c.ContratistaRepresentante,
            c.Estado,
            c.ConceptoContratos.Sum(x => x.Importe),
            c.NombreObra,
            c.ResidenteNombre,
            c.SupervisorExternoNombre,
            c.SuperintendenteNombre,
            c.ConceptoContratos
                .Select(x => new ConceptoContratoDto(
                    x.Id,
                    x.Clave,
                    x.Descripcion,
                    x.UnidadMedida,
                    x.CantidadContratada,
                    x.PrecioUnitario,
                    x.Importe
                ))
                .ToList(),
            c.Garantias
                .Select(x => new GarantiaDto(
                    x.Id,
                    x.Tipo,
                    x.Monto,
                    x.Porcentaje,
                    x.Vigencia,
                    x.Estado
                ))
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
        {
            query = query.Where(c => c.DependenciaContratante == dependencia);
        }

        if (estado.HasValue)
        {
            query = query.Where(c => c.Estado == estado.Value);
        }

        var contratos = await query
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync(ct);

        var resultado = new List<ContratoResumenDto>();

        foreach (var contrato in contratos)
        {
            var montoEstimado = await _context.EstimacionConceptos
                .Where(ec =>
                    ec.Estimacion != null &&
                    ec.Estimacion.ContratoId == contrato.Id &&
                    (
                        ec.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia
                    ))
                .SumAsync(ec => ec.Importe, ct);

            var montoPagado = await _context.EstimacionPagos
                .Where(p =>
                    p.Estimacion != null &&
                    p.Estimacion.ContratoId == contrato.Id)
                .SumAsync(p => p.MontoPagado, ct);

            resultado.Add(new ContratoResumenDto(
                contrato.Id,
                contrato.NumeroContrato,
                contrato.MontoContratado,
                montoEstimado,
                montoPagado,
                contrato.Estado
            ));
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
        var contratoExiste = await _context.Contratos
            .AnyAsync(c => c.Id == contratoId, ct);

        if (!contratoExiste)
            throw new InvalidOperationException("Contrato no encontrado.");

        var existente = await _context.ConceptosContrato
            .FirstOrDefaultAsync(c =>
                c.ContratoId == contratoId &&
                c.Clave == dto.Clave, ct);

        if (existente is not null)
        {
            existente.Descripcion = dto.Descripcion;
            existente.UnidadMedida = dto.UnidadMedida;
            existente.CantidadContratada = dto.CantidadContratada;
            existente.PrecioUnitario = dto.PrecioUnitario;
            existente.Importe = dto.CantidadContratada * dto.PrecioUnitario;
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
                PrecioUnitario = dto.PrecioUnitario,
                Importe = dto.CantidadContratada * dto.PrecioUnitario
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

        contrato.MontoContratado = nuevoMonto;

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
        contrato.MontoContratado = dto.MontoContratado;
        contrato.FechaInicio = dto.FechaInicio;
        contrato.FechaTermino = dto.FechaTermino;
        contrato.PeriodoEstimacion = dto.PeriodoEstimacion;
        contrato.DependenciaContratante = dto.DependenciaContratante;
        contrato.ContratistaEmpresa = dto.ContratistaEmpresa;
        contrato.ContratistaRepresentante = dto.ContratistaRepresentante;
        contrato.ResidenteNombre = dto.ResidenteNombre;
        contrato.SupervisorExternoNombre = dto.SupervisorExternoNombre;
        contrato.SuperintendenteNombre = dto.SuperintendenteNombre;

        await _context.SaveChangesAsync(ct);
    }
}