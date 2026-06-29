using Fepi.Api.DTOs;
using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IAlertaUsuarioService
{
    Task<int> CrearAlertaAsync(int usuarioId, int? contratoId, string entidadRelacionada, int? entidadId,
        string titulo, string mensaje, TipoAlerta tipo, PrioridadAlerta prioridad, CancellationToken ct = default);

    Task<List<AlertaUsuarioResponse>> ObtenerAlertasUsuarioAsync(int usuarioId, CancellationToken ct = default);

    Task<List<AlertaUsuarioResponse>> ObtenerAlertasNoLeidasUsuarioAsync(int usuarioId, CancellationToken ct = default);

    Task MarcarComoLeidaAsync(int alertaId, CancellationToken ct = default);

    Task CrearAlertasSegunEventoAsync(TipoAlerta tipo, int contratoId, int entidadId, CancellationToken ct = default);
}
