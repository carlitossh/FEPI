using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

/// <summary>SV-02 — Control de Avance Físico y Curva S. Cubre HU-07, HU-08.</summary>
[ApiController]
[Route("api/avance")]
public class AvanceController : ControllerBase
{
    private readonly IAvanceService _service;
    public AvanceController(IAvanceService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> RegistrarAvance([FromBody] RegistrarAvanceDto dto, CancellationToken ct)
    {
        await _service.RegistrarAvanceDiarioAsync(dto, ct);
        return NoContent();
    }

    [HttpGet("contrato/{contratoId:int}/curva-s")]
    public async Task<ActionResult<CurvaSDto>> ObtenerCurvaS(int contratoId, CancellationToken ct)
        => Ok(await _service.ObtenerCurvaSAsync(contratoId, ct));

    [HttpGet("dependencia/resumen")]
    public async Task<ActionResult<List<AvanceResumenContratoDto>>> ObtenerResumenPorDependencia([FromQuery] string dependencia, CancellationToken ct)
        => Ok(await _service.ObtenerResumenPorDependenciaAsync(dependencia, ct));

    [HttpGet("/api/contratos/{contratoId:int}/avance")]
    public async Task<ActionResult<CurvaSDto>> ObtenerAvance(
        int contratoId,
        CancellationToken ct)
    {
        return Ok(await _service.ObtenerCurvaSAsync(contratoId, ct));
    }

    [HttpPost("/api/contratos/{contratoId:int}/avance")]
    public async Task<IActionResult> RegistrarAvanceContrato(
        int contratoId,
        [FromBody] RegistrarAvanceDto dto,
        CancellationToken ct)
    {
        if (dto.ContratoId != contratoId)
            return BadRequest("El contrato de la ruta no coincide con el del cuerpo.");

        await _service.RegistrarAvanceDiarioAsync(dto, ct);

        return NoContent();
    }
}
