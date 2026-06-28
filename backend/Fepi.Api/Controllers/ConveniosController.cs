using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

/// <summary>SV-04 — Convenios Modificatorios. Cubre HU-15 a HU-18.</summary>
[ApiController]
[Route("api/convenios")]
public class ConveniosController : ControllerBase
{
    private readonly IConvenioService _service;
    public ConveniosController(IConvenioService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<int>> Solicitar([FromBody] CrearConvenioDto dto, CancellationToken ct)
    {
        var id = await _service.SolicitarAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id }, id);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ConvenioDetalleDto>> ObtenerDetalle(int id, CancellationToken ct)
        => Ok(await _service.ObtenerDetalleAsync(id, ct));

    [HttpGet("contrato/{contratoId:int}")]
    public async Task<ActionResult<List<ConvenioResumenDto>>> ListarPorContrato(int contratoId, CancellationToken ct)
        => Ok(await _service.ListarPorContratoAsync(contratoId, ct));

    [HttpPost("{id:int}/revisar")]
    public async Task<IActionResult> Revisar(int id, [FromBody] RevisarConvenioDto dto, CancellationToken ct)
    {
        await _service.RevisarAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/promover")]
    public async Task<IActionResult> Promover(int id, [FromBody] PromoverConvenioDto dto, CancellationToken ct)
    {
        await _service.PromoverAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/resolver")]
    public async Task<IActionResult> Resolver(int id, [FromBody] ResolverConvenioDto dto, CancellationToken ct)
    {
        await _service.ResolverAsync(id, dto, ct);
        return NoContent();
    }

    [HttpGet("/api/contratos/{contratoId:int}/convenios")]
    public async Task<ActionResult<List<ConvenioResumenDto>>> ListarPorContratoAlias(int contratoId, CancellationToken ct)
        => Ok(await _service.ListarPorContratoAsync(contratoId, ct));

    [HttpPost("/api/contratos/{contratoId:int}/convenios")]
    public async Task<ActionResult<int>> SolicitarViaContrato(int contratoId, [FromBody] CrearConvenioDto dto, CancellationToken ct)
    {
        var id = await _service.SolicitarAsync(dto with { ContratoId = contratoId }, ct);
        return Ok(id);
    }
}
