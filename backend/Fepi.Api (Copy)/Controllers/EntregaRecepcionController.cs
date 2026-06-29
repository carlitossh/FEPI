using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/entrega-recepcion")]
public class EntregaRecepcionController : ControllerBase
{
    private readonly IEntregaRecepcionService _service;
    public EntregaRecepcionController(IEntregaRecepcionService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Iniciar([FromBody] IniciarEntregaRecepcionDto dto, CancellationToken ct)
    {
        await _service.IniciarAsync(dto, ct);
        return NoContent();
    }

    [HttpGet("contrato/{contratoId:int}")]
    public async Task<ActionResult<EntregaRecepcionDto>> Obtener(int contratoId, CancellationToken ct)
    {
        var result = await _service.ObtenerAsync(contratoId, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
