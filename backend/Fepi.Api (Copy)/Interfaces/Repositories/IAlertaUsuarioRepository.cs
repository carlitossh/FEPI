using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IAlertaUsuarioRepository
{
    Task<List<AlertaUsuario>> ObtenerPorUsuarioAsync(int usuarioId, CancellationToken ct = default);
    Task<List<AlertaUsuario>> ObtenerNoLeidasPorUsuarioAsync(int usuarioId, CancellationToken ct = default);
    Task<AlertaUsuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(AlertaUsuario alerta, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
