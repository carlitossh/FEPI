using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IEntregaRecepcionService
{
    Task IniciarAsync(IniciarEntregaRecepcionDto dto, CancellationToken ct = default);
    Task<EntregaRecepcionDto> ObtenerAsync(int contratoId, CancellationToken ct = default);
    Task<FiniquitoDto> EmitirFiniquitoAsync(int contratoId, EmitirFiniquitoDto dto, CancellationToken ct = default);
    Task<FiniquitoDto> ObtenerFiniquitoAsync(int contratoId, CancellationToken ct = default);
}