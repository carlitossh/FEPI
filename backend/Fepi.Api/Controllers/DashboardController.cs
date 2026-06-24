using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
        => _service = service;

    [HttpGet("contrato/{contratoId:int}")]
    public async Task<ActionResult<DashboardContratoDto>> ObtenerPorContrato(
        int contratoId,
        CancellationToken ct)
        => Ok(await _service.ObtenerPorContratoAsync(contratoId, ct));
}