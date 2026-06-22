using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

/// <summary>SV-03 — Bitácora Electrónica. Cubre HU-09 a HU-14.</summary>
[ApiController]
[Route("api/bitacora")]
public class BitacoraController : ControllerBase
{
    private readonly IBitacoraService _service;
    public BitacoraController(IBitacoraService service) => _service = service;

    [HttpPost("abrir")]
    public async Task<ActionResult<CaratulaBitacoraDto>> Abrir([FromBody] AbrirBitacoraDto dto, CancellationToken ct)
        => Ok(await _service.AbrirBitacoraAsync(dto, ct));

    [HttpPost("notas")]
    public async Task<ActionResult<BitacoraNotaDto>> CrearNota([FromBody] CrearNotaBitacoraDto dto, CancellationToken ct)
        => Ok(await _service.CrearNotaAsync(dto, ct));

    [HttpPost("notas/{notaId:int}/firmar")]
    public async Task<IActionResult> Firmar(int notaId, [FromBody] FirmarNotaDto dto, CancellationToken ct)
    {
        await _service.FirmarNotaAsync(notaId, dto, ct);
        return NoContent();
    }

    [HttpGet("notas/buscar")]
    public async Task<ActionResult<List<BitacoraNotaDto>>> Buscar(
        [FromQuery] int BitacoraId, [FromQuery] string? asunto,
        [FromQuery] DateOnly? fechaInicio, [FromQuery] DateOnly? fechaFin, [FromQuery] int? actorId, CancellationToken ct)
        => Ok(await _service.BuscarNotasAsync(BitacoraId, asunto, fechaInicio, fechaFin, actorId, ct));

    [HttpPost("minutas")]
    public async Task<IActionResult> CrearMinuta([FromBody] CrearMinutaDto dto, CancellationToken ct)
    {
        await _service.CrearMinutaAsync(dto, ct);
        return NoContent();
    }

    [HttpPost("incidencias")]
    public async Task<ActionResult<int>> CrearIncidencia([FromBody] CrearIncidenciaDto dto, CancellationToken ct)
        => Ok(await _service.CrearIncidenciaAsync(dto, ct));

    [HttpPost("incidencias/generar-nota")]
    public async Task<ActionResult<BitacoraNotaDto>> GenerarNotaDesdeIncidencia([FromBody] GenerarNotaDesdeIncidenciaDto dto, CancellationToken ct)
        => Ok(await _service.GenerarNotaDesdeIncidenciaAsync(dto, ct));
}
