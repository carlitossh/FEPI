using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDto?> ObtenerPorIdAsync(int usuarioId, CancellationToken ct = default);
    Task<List<UsuarioContratoDto>> ObtenerRolesYContratosAsync(int usuarioId, CancellationToken ct = default);
}