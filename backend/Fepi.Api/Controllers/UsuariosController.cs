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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(
        int id,
        CancellationToken ct)
    {
        var usuario = await _service.ObtenerPorIdAsync(id, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpGet("{id:int}/roles")]
    public async Task<ActionResult<List<UsuarioContratoDto>>> ObtenerRoles(
        int id,
        CancellationToken ct)
        => Ok(await _service.ObtenerRolesYContratosAsync(id, ct));
}