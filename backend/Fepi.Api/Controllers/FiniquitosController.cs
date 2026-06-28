using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
public class FiniquitosController : ControllerBase
{
    private readonly IFiniquitoContratoService _service;

    public FiniquitosController(IFiniquitoContratoService service) => _service = service;

    /// <summary>Inicia el cierre del contrato, crea FiniquitoContrato y nota de cierre en bitácora.</summary>
    [HttpPost("api/contratos/{contratoId:int}/finiquito/iniciar-cierre")]
    public async Task<ActionResult<IniciarCierreContratoResponse>> IniciarCierre(
        int contratoId, [FromBody] IniciarCierreContratoRequest dto, CancellationToken ct)
    {
        var result = await _service.IniciarCierreContratoAsync(contratoId, dto.UsuarioId, ct);
        return Ok(result);
    }

    /// <summary>Obtiene el finiquito del contrato con su estado actual.</summary>
    [HttpGet("api/contratos/{contratoId:int}/finiquito")]
    public async Task<ActionResult<FiniquitoContratoResponse>> Obtener(int contratoId, CancellationToken ct)
    {
        var result = await _service.ObtenerFiniquitoPorContratoAsync(contratoId, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Calcula el resumen financiero del finiquito sin guardarlo.</summary>
    [HttpGet("api/contratos/{contratoId:int}/finiquito/resumen")]
    public async Task<ActionResult<FiniquitoResumenResponse>> ObtenerResumen(int contratoId, CancellationToken ct)
        => Ok(await _service.CalcularResumenFiniquitoAsync(contratoId, ct));

    /// <summary>Registra el finiquito con montos definitivos. La nota de cierre debe estar firmada.</summary>
    [HttpPost("api/finiquitos/{id:int}/registrar")]
    public async Task<ActionResult<FiniquitoContratoResponse>> Registrar(
        int id, [FromBody] RegistrarFiniquitoRequest dto, CancellationToken ct)
    {
        var result = await _service.RegistrarFiniquitoAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>Aprueba el finiquito registrado.</summary>
    [HttpPost("api/finiquitos/{id:int}/aprobar")]
    public async Task<ActionResult<FiniquitoContratoResponse>> Aprobar(int id, CancellationToken ct)
    {
        var result = await _service.AprobarFiniquitoAsync(id, ct);
        return Ok(result);
    }

    /// <summary>Cierra definitivamente el contrato. El finiquito debe estar aprobado.</summary>
    [HttpPost("api/finiquitos/{id:int}/cerrar")]
    public async Task<IActionResult> Cerrar(int id, CancellationToken ct)
    {
        await _service.CerrarContratoAsync(id, ct);
        return NoContent();
    }
}
