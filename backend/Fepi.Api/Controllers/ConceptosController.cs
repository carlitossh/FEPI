using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/contratos/{contratoId:int}/conceptos")]
public class ConceptosController : ControllerBase
{
    private readonly IConceptosService _service;

    public ConceptosController(IConceptosService service)
    {
        _service = service;
    }

    [HttpGet("secciones")]
    public async Task<ActionResult<List<SeccionConceptoResponse>>> ListarSecciones(int contratoId, CancellationToken ct)
    {
        return Ok(await _service.ListarSeccionesAsync(contratoId, ct));
    }

    [HttpPost("secciones")]
    public async Task<IActionResult> CrearSeccion(int contratoId, [FromBody] CrearSeccionRequest dto, CancellationToken ct)
    {
        var id = await _service.CrearSeccionAsync(contratoId, dto, ct);
        var secciones = await _service.ListarSeccionesAsync(contratoId, ct);
        var seccion = secciones.FirstOrDefault(s => s.Id == id);
        return CreatedAtAction(nameof(ListarSecciones), new { contratoId }, seccion);
    }

    [HttpPut("{conceptoId:int}")]
    public async Task<IActionResult> ActualizarConcepto(
        int contratoId, int conceptoId, [FromBody] CrearConceptoRequest dto, CancellationToken ct)
    {
        await _service.ActualizarConceptoAsync(conceptoId, dto, ct);
        return NoContent();
    }

    [HttpPost("secciones/{seccionId:int}/conceptos")]
    public async Task<IActionResult> AgregarConcepto(
        int contratoId, int seccionId, [FromBody] CrearConceptoRequest dto, CancellationToken ct)
    {
        var id = await _service.CrearConceptoAsync(contratoId, dto, ct);
        return CreatedAtAction(nameof(ListarSecciones), new { contratoId }, new { id });
    }

    [HttpDelete("{conceptoId:int}")]
    public async Task<IActionResult> DesactivarConcepto(int contratoId, int conceptoId, CancellationToken ct)
    {
        await _service.DesactivarConceptoAsync(conceptoId, ct);
        return NoContent();
    }
}
