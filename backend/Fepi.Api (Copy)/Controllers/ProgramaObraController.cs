using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/contratos/{contratoId:int}/programa-obra")]
public class ProgramaObraController : ControllerBase
{
    private readonly IProgramaObraService _service;

    public ProgramaObraController(IProgramaObraService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ProgramaObraResponse>> Obtener(int contratoId, CancellationToken ct)
    {
        return Ok(await _service.ObtenerProgramaAsync(contratoId, ct));
    }

    [HttpPost("secciones")]
    public async Task<IActionResult> AsignarSeccion(int contratoId, [FromBody] CrearProgramaSeccionRequest dto, CancellationToken ct)
    {
        var id = await _service.CrearSeccionProgramaAsync(contratoId, dto, ct);
        return CreatedAtAction(nameof(Obtener), new { contratoId }, new { id });
    }

    [HttpGet("avance-planificado")]
    public async Task<ActionResult<List<AvancePlanificadoPeriodoDto>>> AvancePlanificado(int contratoId, CancellationToken ct)
    {
        return Ok(await _service.ObtenerAvancePlanificadoAsync(contratoId, ct));
    }
}
