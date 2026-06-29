using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IConvenioService
{
    Task<int> SolicitarAsync(CrearConvenioDto dto, CancellationToken ct = default);
    Task<ConvenioDetalleDto> ObtenerDetalleAsync(int convenioId, CancellationToken ct = default);
    Task<List<ConvenioResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default);
    Task RevisarAsync(int convenioId, RevisarConvenioDto dto, CancellationToken ct = default);
    Task PromoverAsync(int convenioId, PromoverConvenioDto dto, CancellationToken ct = default);
    Task ResolverAsync(int convenioId, ResolverConvenioDto dto, CancellationToken ct = default);
    Task<AplicarConvenioResultadoDto> AplicarAsync(int convenioId, AplicarConvenioDto dto, CancellationToken ct = default);
    Task<List<ConvenioHistorialDto>> ObtenerHistorialAsync(int convenioId, CancellationToken ct = default);
}
