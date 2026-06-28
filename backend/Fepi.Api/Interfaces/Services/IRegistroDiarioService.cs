using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IRegistroDiarioService
{
    Task<List<RegistroDiarioResponse>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default);
    Task<int> CrearAsync(int contratoId, CrearRegistroDiarioRequest dto, CancellationToken ct = default);
}
