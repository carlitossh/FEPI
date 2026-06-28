using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/alertas")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _alertaService;
    private readonly IAlertaUsuarioService _alertaUsuarioService;

    public AlertasController(IAlertaService alertaService, IAlertaUsuarioService alertaUsuarioService)
    {
        _alertaService = alertaService;
        _alertaUsuarioService = alertaUsuarioService;
    }

    // ── Alertas por rol (sistema legado) ──────────────────────────────────────

    [HttpGet("rol")]
    public async Task<ActionResult<List<AlertaDto>>> ObtenerActivasPorRol(
        [FromQuery] RolSistema rol, [FromQuery] int? contratoId, CancellationToken ct)
        => Ok(await _alertaService.ObtenerActivasPorRolAsync(rol, contratoId, ct));

    // ── Alertas por usuario ───────────────────────────────────────────────────

    /// <summary>Todas las alertas del usuario (leídas y no leídas).</summary>
    [HttpGet]
    public async Task<ActionResult<List<AlertaUsuarioResponse>>> Listar(
        [FromQuery] int usuarioId, CancellationToken ct)
        => Ok(await _alertaUsuarioService.ObtenerAlertasUsuarioAsync(usuarioId, ct));

    /// <summary>Solo alertas no leídas del usuario.</summary>
    [HttpGet("no-leidas")]
    public async Task<ActionResult<List<AlertaUsuarioResponse>>> NoLeidas(
        [FromQuery] int usuarioId, CancellationToken ct)
        => Ok(await _alertaUsuarioService.ObtenerAlertasNoLeidasUsuarioAsync(usuarioId, ct));

    /// <summary>Marca una alerta como leída.</summary>
    [HttpPost("{id:int}/leer")]
    public async Task<IActionResult> MarcarLeida(int id, CancellationToken ct)
    {
        await _alertaUsuarioService.MarcarComoLeidaAsync(id, ct);
        return NoContent();
    }
}
