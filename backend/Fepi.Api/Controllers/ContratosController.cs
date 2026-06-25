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
    private readonly IEntregaRecepcionService _entregaService;

    public ContratosController(
        IContratoService service,
        IUsuarioService usuarioService,
        IEntregaRecepcionService entregaService)
    {
        _service = service;
        _usuarioService = usuarioService;
        _entregaService = entregaService;
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ActualizarContratoDto dto,
        CancellationToken ct)
    {
        await _service.ActualizarAsync(id, dto, ct);
        return NoContent();
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

    [HttpGet("{id:int}/finiquito")]
    public async Task<ActionResult<FiniquitoDto>> ObtenerFiniquito(int id, CancellationToken ct)
    {
        var result = await _entregaService.ObtenerFiniquitoAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:int}/cierre")]
    public async Task<IActionResult> RegistrarCierre(
        int id,
        [FromBody] RegistrarCierreDto dto,
        CancellationToken ct)
    {
        await _entregaService.IniciarAsync(
            new IniciarEntregaRecepcionDto(id, dto.FechaEntrega, dto.EstadoObraDescripcion, dto.EstadoGarantiasDescripcion, dto.UrlsEvidencia),
            ct);
        return NoContent();
    }
}

public record ActualizarMontoContratoDto(decimal NuevoMonto);