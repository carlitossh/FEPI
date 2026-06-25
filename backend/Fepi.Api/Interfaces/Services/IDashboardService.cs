using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IDashboardService
{
    Task<DashboardContratoDto> ObtenerPorContratoAsync(int contratoId, CancellationToken ct = default);
    Task<CurvaFinancieraDto> ObtenerCurvaFinancieraAsync(int contratoId, CancellationToken ct = default);
    Task<List<VencimientoDto>> ObtenerVencimientosAsync(int contratoId, CancellationToken ct = default);
    Task<BitacoraResumenDto> ObtenerBitacoraResumenAsync(int contratoId, CancellationToken ct = default);
    Task<EstimacionesResumenDto> ObtenerEstimacionesResumenAsync(int contratoId, CancellationToken ct = default);
    Task<ConveniosResumenDto> ObtenerConveniosResumenAsync(int contratoId, CancellationToken ct = default);
    Task<List<ActividadRecienteDto>> ObtenerActividadRecienteAsync(int contratoId, CancellationToken ct = default);
}
