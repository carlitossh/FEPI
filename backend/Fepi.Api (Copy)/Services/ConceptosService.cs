using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class ConceptosService : IConceptosService
{
    private readonly FepiDbContext _context;

    public ConceptosService(FepiDbContext context)
    {
        _context = context;
    }

    public async Task<List<SeccionConceptoResponse>> ListarSeccionesAsync(int contratoId, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .Include(c => c.SeccionesConcepto)
                .ThenInclude(s => s.Conceptos)
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        return contrato.SeccionesConcepto.Select(s =>
        {
            var importeSeccion = s.Conceptos.Where(c => c.Activo).Sum(c => c.PrecioUnitario * c.CantidadContratada);
            var peso = contrato.ImporteSinIVA > 0 ? importeSeccion / contrato.ImporteSinIVA : 0;
            return new SeccionConceptoResponse(
                s.Id, s.ContratoId, s.Nombre, s.Descripcion, importeSeccion, peso,
                s.Conceptos.Select(c => new ConceptoContratoResponse(
                    c.Id, c.ContratoId, c.SeccionConceptoId, c.Clave, c.Descripcion,
                    c.UnidadMedida, c.CantidadContratada, c.PrecioUnitario,
                    c.PrecioUnitario * c.CantidadContratada, c.EsExtraordinario, c.Activo)).ToList());
        }).ToList();
    }

    public async Task<int> CrearSeccionAsync(int contratoId, CrearSeccionRequest dto, CancellationToken ct = default)
    {
        var contratoExiste = await _context.Contratos.AnyAsync(c => c.Id == contratoId, ct);
        if (!contratoExiste)
            throw new InvalidOperationException("Contrato no encontrado.");

        var seccion = new SeccionConcepto
        {
            ContratoId = contratoId,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        _context.SeccionesConcepto.Add(seccion);
        await _context.SaveChangesAsync(ct);
        return seccion.Id;
    }

    public async Task<int> CrearConceptoAsync(int contratoId, CrearConceptoRequest dto, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .Include(c => c.ConceptoContratos)
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var seccionExiste = await _context.SeccionesConcepto
            .AnyAsync(s => s.Id == dto.SeccionConceptoId && s.ContratoId == contratoId, ct);
        if (!seccionExiste)
            throw new InvalidOperationException("Sección no encontrada en este contrato.");

        var nuevoImporte = dto.CantidadContratada * dto.PrecioUnitario;
        var sumaActual = contrato.ConceptoContratos
            .Where(c => c.Activo)
            .Sum(c => c.CantidadContratada * c.PrecioUnitario);

        if (!dto.EsExtraordinario && sumaActual + nuevoImporte > contrato.ImporteSinIVA + 0.01m)
            throw new InvalidOperationException(
                $"Agregar este concepto excedería el importe sin IVA del contrato ({contrato.ImporteSinIVA:N2}). " +
                $"Suma actual: {sumaActual:N2}, nuevo concepto: {nuevoImporte:N2}.");

        var concepto = new ConceptoContrato
        {
            ContratoId = contratoId,
            SeccionConceptoId = dto.SeccionConceptoId,
            Clave = dto.Clave,
            Descripcion = dto.Descripcion,
            UnidadMedida = dto.UnidadMedida,
            CantidadContratada = dto.CantidadContratada,
            PrecioUnitario = dto.PrecioUnitario,
            EsExtraordinario = dto.EsExtraordinario,
            ConvenioModificatorioId = dto.ConvenioModificatorioId,
            Activo = true
        };

        _context.ConceptosContrato.Add(concepto);
        await _context.SaveChangesAsync(ct);
        return concepto.Id;
    }

    public async Task ActualizarConceptoAsync(int conceptoId, CrearConceptoRequest dto, CancellationToken ct = default)
    {
        var concepto = await _context.ConceptosContrato
            .Include(c => c.Contrato)
            .FirstOrDefaultAsync(c => c.Id == conceptoId, ct)
            ?? throw new InvalidOperationException("Concepto no encontrado.");

        var nuevoImporte = dto.CantidadContratada * dto.PrecioUnitario;
        var importeActualDeEsteConcepto = concepto.CantidadContratada * concepto.PrecioUnitario;

        var sumaOtros = await _context.ConceptosContrato
            .Where(c => c.ContratoId == concepto.ContratoId && c.Activo && c.Id != conceptoId)
            .SumAsync(c => c.CantidadContratada * c.PrecioUnitario, ct);

        var importeSinIVA = concepto.Contrato!.ImporteSinIVA;

        if (!concepto.EsExtraordinario && sumaOtros + nuevoImporte > importeSinIVA + 0.01m)
            throw new InvalidOperationException(
                $"Actualizar este concepto excedería el importe sin IVA del contrato ({importeSinIVA:N2}). " +
                $"Suma otros conceptos activos: {sumaOtros:N2}, importe nuevo: {nuevoImporte:N2}.");

        concepto.SeccionConceptoId = dto.SeccionConceptoId;
        concepto.Clave = dto.Clave;
        concepto.Descripcion = dto.Descripcion;
        concepto.UnidadMedida = dto.UnidadMedida;
        concepto.CantidadContratada = dto.CantidadContratada;
        concepto.PrecioUnitario = dto.PrecioUnitario;

        await _context.SaveChangesAsync(ct);
    }

    public async Task DesactivarConceptoAsync(int conceptoId, CancellationToken ct = default)
    {
        var concepto = await _context.ConceptosContrato.FindAsync(new object[] { conceptoId }, ct)
            ?? throw new InvalidOperationException("Concepto no encontrado.");

        concepto.Activo = false;
        await _context.SaveChangesAsync(ct);
    }
}
