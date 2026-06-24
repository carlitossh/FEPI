using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/contratos")]
public class ContratosController : ControllerBase
{
    private readonly IContratoService _service;

    public ContratosController(IContratoService service)
        => _service = service;

    [HttpPost]
    public async Task<ActionResult<int>> Crear(
        [FromBody] CrearContratoDto dto,
        CancellationToken ct)
    {
        var id = await _service.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id }, id);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContratoDetalleDto>> ObtenerDetalle(
        int id,
        CancellationToken ct)
        => Ok(await _service.ObtenerDetalleAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<List<ContratoResumenDto>>> ListarPorDependencia(
        [FromQuery] string dependencia,
        [FromQuery] EstadoContrato? Estado,
        CancellationToken ct)
        => Ok(await _service.ListarPorDependenciaAsync(dependencia, Estado, ct));

    [HttpPut("{id:int}/programa-obra")]
    public async Task<IActionResult> ActualizarProgramaObra(
        int id,
        [FromBody] ActualizarProgramaObraDto dto,
        CancellationToken ct)
    {
        await _service.ActualizarProgramaObraAsync(id, dto, ct);
        return NoContent();
    }
}