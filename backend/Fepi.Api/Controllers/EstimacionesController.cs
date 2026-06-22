using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

/// <summary>SV-01 — Gestión de Estimaciones. Cubre HU-01 a HU-06.</summary>
[ApiController]
[Route("api/estimaciones")]
public class EstimacionesController : ControllerBase
{
    private readonly IEstimacionService _service;
    public EstimacionesController(IEstimacionService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<EstimacionResumenDto>> Crear([FromBody] CrearEstimacionDto dto, CancellationToken ct)
    {
        var result = await _service.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id = result.Id }, result);
    }

    [HttpGet("contrato/{contratoId:int}")]
    public async Task<ActionResult<List<EstimacionResumenDto>>> ListarPorContrato(int contratoId, CancellationToken ct)
        => Ok(await _service.ListarPorContratoAsync(contratoId, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EstimacionDetalleDto>> ObtenerDetalle(int id, CancellationToken ct)
        => Ok(await _service.ObtenerDetalleAsync(id, ct));

    [HttpPut("{id:int}/conceptos")]
    public async Task<IActionResult> ActualizarConceptos(int id, [FromBody] ActualizarConceptosEstimacionDto dto, CancellationToken ct)
    {
        await _service.ActualizarConceptosAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/notas-bitacora")]
    public async Task<IActionResult> VincularNotas(int id, [FromBody] VincularNotasDto dto, CancellationToken ct)
    {
        await _service.VincularNotasBitacoraAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/enviar")]
    public async Task<IActionResult> Enviar(int id, [FromQuery] int usuarioId, CancellationToken ct)
    {
        await _service.EnviarAsync(id, usuarioId, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/observaciones")]
    public async Task<ActionResult<ObservacionDto>> AgregarObservacion(int id, [FromBody] CrearObservacionDto dto, [FromQuery] int usuarioId, CancellationToken ct)
        => Ok(await _service.AgregarObservacionAsync(id, dto, usuarioId, ct));

    [HttpPost("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoEstimacionDto dto, CancellationToken ct)
    {
        await _service.CambiarEstadoAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/pago")]
    public async Task<IActionResult> RegistrarPago(int id, [FromBody] RegistrarPagoEstimacionDto dto, CancellationToken ct)
    {
        await _service.RegistrarPagoAsync(id, dto, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/historial")]
    public async Task<ActionResult<List<EstimacionHistorialDto>>> ObtenerHistorial(int id, CancellationToken ct)
        => Ok(await _service.ObtenerHistorialAsync(id, ct));
}
