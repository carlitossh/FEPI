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
    {
        _service = service;
    }

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
    {
        return Ok(await _service.ObtenerDetalleAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<List<ContratoResumenDto>>> Listar(
        [FromQuery] string? dependencia,
        [FromQuery] EstadoContrato? estado,
        CancellationToken ct)
    {
        return Ok(await _service.ListarPorDependenciaAsync(dependencia, estado, ct));
    }

    [HttpPut("{id:int}/programa-obra")]
    public async Task<IActionResult> ActualizarProgramaObra(
        int id,
        [FromBody] ActualizarProgramaObraDto dto,
        CancellationToken ct)
    {
        await _service.ActualizarProgramaObraAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPut("{id:int}/conceptos")]
    public async Task<IActionResult> AgregarOActualizarConcepto(
        int id,
        [FromBody] ConceptoContratoInputDto dto,
        CancellationToken ct)
    {
        await _service.AgregarOActualizarConceptoCatalogoAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/monto")]
    public async Task<IActionResult> ActualizarMonto(
        int id,
        [FromBody] ActualizarMontoContratoDto dto,
        CancellationToken ct)
    {
        await _service.ActualizarMontoContratadoAsync(id, dto.NuevoMonto, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/conceptos")]
    public async Task<ActionResult<List<ConceptoContratoDto>>> ObtenerConceptos(
        int id,
        CancellationToken ct)
    {
        var contrato = await _service.ObtenerDetalleAsync(id, ct);
        return Ok(contrato.ConceptoContratos);
    }
}

public record ActualizarMontoContratoDto(decimal NuevoMonto);