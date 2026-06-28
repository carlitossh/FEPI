using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/contratos/{contratoId:int}/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
        => _service = service;

    [HttpGet]
    public async Task<ActionResult<DashboardContratoDto>> Obtener(
        int contratoId,
        CancellationToken ct)
        => Ok(await _service.ObtenerPorContratoAsync(contratoId, ct));
}
