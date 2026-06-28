using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

/// <summary>SV-02 — Control de Avance Físico y Curva S. Cubre HU-07, HU-08.</summary>
[ApiController]
[Route("api/contratos/{contratoId:int}/avance")]
public class AvanceController : ControllerBase
{
    private readonly IAvanceService _service;
    public AvanceController(IAvanceService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<CurvaSDto>> Obtener(int contratoId, CancellationToken ct)
        => Ok(await _service.ObtenerCurvaSAsync(contratoId, ct));

    [HttpPost]
    public async Task<IActionResult> Registrar(
        int contratoId,
        [FromBody] RegistrarAvanceDto dto,
        CancellationToken ct)
    {
        if (dto.ContratoId != contratoId)
            return BadRequest("El contrato de la ruta no coincide con el del cuerpo.");

        await _service.RegistrarAvanceDiarioAsync(dto, ct);
        return NoContent();
    }

    /// <summary>Resumen de avance por dependencia (agregado multi-contrato).</summary>
    [HttpGet("/api/contratos/avance/resumen")]
    public async Task<ActionResult<List<AvanceResumenContratoDto>>> ObtenerResumenPorDependencia(
        [FromQuery] string dependencia,
        CancellationToken ct)
        => Ok(await _service.ObtenerResumenPorDependenciaAsync(dependencia, ct));
}
