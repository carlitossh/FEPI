using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IDashboardService
{
    Task<DashboardContratoDto> ObtenerPorContratoAsync(int contratoId, CancellationToken ct = default);
}