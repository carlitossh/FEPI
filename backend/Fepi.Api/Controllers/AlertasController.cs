using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/alertas")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _service;
    public AlertasController(IAlertaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<AlertaDto>>> ObtenerActivasPorRol([FromQuery] RolSistema rol, [FromQuery] int? contratoId, CancellationToken ct)
        => Ok(await _service.ObtenerActivasPorRolAsync(rol, contratoId, ct));
}