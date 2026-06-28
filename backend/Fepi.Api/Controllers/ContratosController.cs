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
    private readonly IUsuarioService _usuarioService;

    public ContratosController(IContratoService service, IUsuarioService usuarioService)
    {
        _service = service;
        _usuarioService = usuarioService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Crear(
        [FromBody] CrearContratoDto dto,
        CancellationToken ct)
    {
        var id = await _service.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerResumen), new { id },
            new ApiResponse<int>(true, "Contrato creado correctamente.", id));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContratoDetalleDto>> ObtenerResumen(
        int id,
        CancellationToken ct)
    {
        return Ok(await _service.ObtenerDetalleAsync(id, ct));
    }

    [HttpGet("{id:int}/detalle")]
    public async Task<ActionResult<ApiResponse<ContratoDetalleCompletoDto>>> ObtenerDetalle(
        int id,
        CancellationToken ct)
    {
        var detalle = await _service.ObtenerDetalleCompletoAsync(id, ct);
        return Ok(new ApiResponse<ContratoDetalleCompletoDto>(true, "OK", detalle));
    }

    [HttpGet]
    public async Task<ActionResult<List<ContratoResumenDto>>> Listar(
        [FromQuery] string? dependencia,
        [FromQuery] EstadoContrato? estado,
        CancellationToken ct)
    {
        return Ok(await _service.ListarPorDependenciaAsync(dependencia, estado, ct));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ActualizarContratoDto dto,
        CancellationToken ct)
    {
        await _service.ActualizarAsync(id, dto, ct);
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

    [HttpGet("{id:int}/usuarios")]
    public async Task<ActionResult<List<UsuarioContratoDetalleDto>>> ListarUsuarios(
        int id,
        CancellationToken ct)
        => Ok(await _usuarioService.ListarPorContratoAsync(id, ct));

    [HttpPost("{id:int}/usuarios")]
    public async Task<ActionResult<UsuarioContratoDetalleDto>> InvitarUsuario(
        int id,
        [FromBody] InvitarUsuarioContratoDto dto,
        CancellationToken ct)
    {
        var result = await _usuarioService.InvitarAsync(id, dto, ct);
        return Ok(result);
    }
}

