using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IProgramaObraService
{
    Task<int> CrearSeccionProgramaAsync(int contratoId, CrearProgramaSeccionRequest dto, CancellationToken ct = default);
    Task<ProgramaObraResponse> ObtenerProgramaAsync(int contratoId, CancellationToken ct = default);
    Task<List<AvancePlanificadoPeriodoDto>> ObtenerAvancePlanificadoAsync(int contratoId, CancellationToken ct = default);
}
