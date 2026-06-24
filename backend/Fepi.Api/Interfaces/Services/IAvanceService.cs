using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IAvanceService
{
    Task RegistrarAvanceDiarioAsync(RegistrarAvanceDto dto, CancellationToken ct = default);
    Task<CurvaSDto> ObtenerCurvaSAsync(int contratoId, CancellationToken ct = default);
    Task<List<AvanceResumenContratoDto>> ObtenerResumenPorDependenciaAsync(string dependencia, CancellationToken ct = default);
}