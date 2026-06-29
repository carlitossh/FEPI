using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
        => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UsuarioDto>>>> Listar(CancellationToken ct)
    {
        var usuarios = await _service.ListarAsync(ct);
        return Ok(new ApiResponse<List<UsuarioDto>>(true, "OK", usuarios));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(
        int id,
        CancellationToken ct)
    {
        var usuario = await _service.ObtenerPorIdAsync(id, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> Crear(
        [FromBody] CrearUsuarioRequest dto,
        CancellationToken ct)
    {
        var usuario = await _service.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario.Id },
            new ApiResponse<UsuarioDto>(true, "Usuario creado correctamente.", usuario));
    }

    [HttpGet("{id:int}/roles")]
    public async Task<ActionResult<List<UsuarioContratoDto>>> ObtenerRoles(
        int id,
        CancellationToken ct)
        => Ok(await _service.ObtenerRolesYContratosAsync(id, ct));

    [HttpPatch("{id:int}/rol")]
    public async Task<IActionResult> ActualizarRol(
        int id,
        [FromBody] ActualizarRolDto dto,
        CancellationToken ct)
    {
        await _service.ActualizarRolAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/suspender")]
    public async Task<IActionResult> Suspender(int id, CancellationToken ct)
    {
        await _service.SuspenderAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken ct)
    {
        await _service.ActivarAsync(id, ct);
        return NoContent();
    }
}
