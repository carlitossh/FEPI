using Fepi.Api.DTOs;
using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IAlertaService
{
    Task EmitirAsync(int contratoId, TipoAlerta tipo, int entidadId, string entidadTipo, RolSistema rolDestino, string mensaje, CancellationToken ct = default);
    Task ResolverAsync(TipoAlerta tipo, int entidadId, CancellationToken ct = default);
    Task<List<AlertaDto>> ObtenerActivasPorRolAsync(RolSistema rol, int? contratoId = null, CancellationToken ct = default);
}