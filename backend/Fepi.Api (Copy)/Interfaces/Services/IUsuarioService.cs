using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ListarAsync(CancellationToken ct = default);
    Task<UsuarioDto?> ObtenerPorIdAsync(int usuarioId, CancellationToken ct = default);
    Task<UsuarioDto> CrearAsync(CrearUsuarioRequest dto, CancellationToken ct = default);
    Task<UsuarioLoginResponse> LoginAsync(UsuarioLoginRequest dto, CancellationToken ct = default);
    Task<List<UsuarioContratoDto>> ObtenerRolesYContratosAsync(int usuarioId, CancellationToken ct = default);
    Task<List<UsuarioContratoDetalleDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default);
    Task<UsuarioContratoDetalleDto> InvitarAsync(int contratoId, InvitarUsuarioContratoDto dto, CancellationToken ct = default);
    Task ActualizarRolAsync(int usuarioId, ActualizarRolDto dto, CancellationToken ct = default);
    Task SuspenderAsync(int usuarioId, CancellationToken ct = default);
    Task ActivarAsync(int usuarioId, CancellationToken ct = default);
}