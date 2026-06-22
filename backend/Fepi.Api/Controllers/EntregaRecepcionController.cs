using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

/// <summary>SV-05 — Entrega-Recepción y Finiquito. Cubre HU-19, HU-20.</summary>
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
        => Ok(await _service.ObtenerAsync(contratoId, ct));

    [HttpPost("contrato/{contratoId:int}/finiquito")]
    public async Task<ActionResult<FiniquitoDto>> EmitirFiniquito(int contratoId, [FromBody] EmitirFiniquitoDto dto, CancellationToken ct)
        => Ok(await _service.EmitirFiniquitoAsync(contratoId, dto, ct));

    [HttpGet("contrato/{contratoId:int}/finiquito")]
    public async Task<ActionResult<FiniquitoDto>> ObtenerFiniquito(int contratoId, CancellationToken ct)
        => Ok(await _service.ObtenerFiniquitoAsync(contratoId, ct));
}
