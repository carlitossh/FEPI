using Fepi.Api.DTOs;
using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IContratoService
{
    Task<int> CrearAsync(CrearContratoDto dto, CancellationToken ct = default);

    Task<ContratoDetalleDto> ObtenerDetalleAsync(int contratoId, CancellationToken ct = default);

    Task<List<ContratoResumenDto>> ListarPorDependenciaAsync(
        string? dependencia,
        EstadoContrato? estado,
        CancellationToken ct = default);

    Task ActualizarProgramaObraAsync(
        int contratoId,
        ActualizarProgramaObraDto dto,
        CancellationToken ct = default);

    Task AgregarOActualizarConceptoCatalogoAsync(
        int contratoId,
        ConceptoContratoInputDto dto,
        CancellationToken ct = default);

    Task ActualizarMontoContratadoAsync(
        int contratoId,
        decimal nuevoImporteSinIVA,
        decimal ivaPorcentaje,
        CancellationToken ct = default);

    Task ActualizarAsync(int contratoId, ActualizarContratoDto dto, CancellationToken ct = default);

    Task<ContratoDetalleCompletoDto> ObtenerDetalleCompletoAsync(int contratoId, CancellationToken ct = default);
}
