using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class ContratoService : IContratoService
{
    private readonly IContratoRepository _contratoRepo;
    private readonly FepiDbContext _context;

    public ContratoService(IContratoRepository contratoRepo, FepiDbContext context)
    {
        _contratoRepo = contratoRepo;
        _context = context;
    }

    public async Task<int> CrearAsync(CrearContratoDto dto, CancellationToken ct = default)
    {
        var contrato = new Contrato
        {
            Id = int.Newint(), NumeroContrato = dto.NumeroContrato, Tipo = dto.Tipo, MontoContratado = dto.MontoContratado,
            FechaInicio = dto.FechaInicio, FechaTermino = dto.FechaTermino, PeriodoEstimacion = dto.PeriodoEstimacion,
            DependenciaContratante = dto.DependenciaContratante, ContratistaEmpresa = dto.ContratistaEmpresa,
            ContratistaRepresentante = dto.ContratistaRepresentante
        };

        foreach (var c in dto.ConceptoContratos)
            contrato.ConceptoContratos.Add(new ConceptoContrato
            {
                Id = int.Newint(), ContratoId = contrato.Id, Clave = c.Clave, Descripcion = c.Descripcion,
                UnidadMedida = c.UnidadMedida, CantidadContratada = c.CantidadContratada, PrecioUnitario = c.PrecioUnitario,
                Importe = c.CantidadContratada * c.PrecioUnitario
            });

        foreach (var p in dto.ProgramaObra)
            contrato.ProgramaObra.Add(new ProgramaObraItem
            {
                Id = int.Newint(), ContratoId = contrato.Id, Periodo = p.Periodo,
                PorcentajeProgramado = p.PorcentajeProgramado, MontoProgramado = p.MontoProgramado
            });

        foreach (var g in dto.Garantias)
            contrato.Garantias.Add(new Garantia
            {
                Id = int.Newint(), ContratoId = contrato.Id, Tipo = g.Tipo, Monto = g.Monto,
                Porcentaje = g.Porcentaje, Vigencia = g.Vigencia
            });

        await _contratoRepo.AddAsync(contrato, ct);
        await _contratoRepo.SaveChangesAsync(ct);
        return contrato.Id;
    }

    public async Task<ContratoDetalleDto> ObtenerDetalleAsync(int contratoId, CancellationToken ct = default)
    {
        var c = await _contratoRepo.GetConCatalogoYGarantiasAsync(contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        return new ContratoDetalleDto(c.Id, c.NumeroContrato, c.Tipo, c.MontoContratado, c.FechaInicio, c.FechaTermino,
            c.DependenciaContratante, c.ContratistaEmpresa, c.Estado, c.ConceptoContratos.Sum(x => x.Importe),
            c.ConceptoContratos.Select(x => new ConceptoContratoDto(x.Id, x.Clave, x.Descripcion, x.UnidadMedida, x.CantidadContratada, x.PrecioUnitario, x.Importe)).ToList(),
            c.Garantias.Select(x => new GarantiaDto(x.Id, x.Tipo, x.Monto, x.Porcentaje, x.Vigencia, x.Estado)).ToList());
    }

    public async Task<List<ContratoResumenDto>> ListarPorDependenciaAsync(string dependencia, EstadoContrato? Estado, CancellationToken ct = default)
    {
        var contratos = await _contratoRepo.GetByDependenciaAsync(dependencia, Estado, ct);
        var resultado = new List<ContratoResumenDto>();

        foreach (var c in contratos)
        {
            var montoEstimado = await _context.Estimaciones
                .Where(e => e.ContratoId == c.Id && (e.Estado == EstadoEstimacion.Aprobada || e.Estado == EstadoEstimacion.Pagada))
                .Include(e => e.Conceptos).SelectMany(e => e.Conceptos).SumAsync(x => x.Importe, ct);

            var montoPagado = await _context.EstimacionPagos
                .Where(p => p.Estimacion!.ContratoId == c.Id)
                .SumAsync(p => p.MontoPagado, ct);

            resultado.Add(new ContratoResumenDto(c.Id, c.NumeroContrato, c.MontoContratado, montoEstimado, montoPagado, c.Estado));
        }

        return resultado;
    }

    public async Task ActualizarProgramaObraAsync(int contratoId, ActualizarProgramaObraDto dto, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos.Include(c => c.ProgramaObra).FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        _context.ProgramaObraItems.RemoveRange(contrato.ProgramaObra);

        foreach (var p in dto.ProgramaObra)
            contrato.ProgramaObra.Add(new ProgramaObraItem
            {
                Id = int.Newint(), ContratoId = contratoId, Periodo = p.Periodo,
                PorcentajeProgramado = p.PorcentajeProgramado, MontoProgramado = p.MontoProgramado
            });

        await _context.SaveChangesAsync(ct);
    }

    public async Task AgregarOActualizarConceptoCatalogoAsync(int contratoId, ConceptoContratoInputDto dto, CancellationToken ct = default)
    {
        var existente = await _context.ConceptoContratos.FirstOrDefaultAsync(c => c.ContratoId == contratoId && c.Clave == dto.Clave, ct);

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
            _context.ConceptoContratos.Add(new ConceptoContrato
            {
                Id = int.Newint(), ContratoId = contratoId, Clave = dto.Clave, Descripcion = dto.Descripcion,
                UnidadMedida = dto.UnidadMedida, CantidadContratada = dto.CantidadContratada, PrecioUnitario = dto.PrecioUnitario,
                Importe = dto.CantidadContratada * dto.PrecioUnitario
            });
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task ActualizarMontoContratadoAsync(int contratoId, decimal nuevoMonto, CancellationToken ct = default)
    {
        var contrato = await _contratoRepo.GetByIdAsync(contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");
        contrato.MontoContratado = nuevoMonto;
        await _contratoRepo.SaveChangesAsync(ct);
    }
}
