using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/contratos/{contratoId:int}/registros-diarios")]
public class RegistrosDiariosController : ControllerBase
{
    private readonly IRegistroDiarioService _service;

    public RegistrosDiariosController(IRegistroDiarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<RegistroDiarioResponse>>> Listar(int contratoId, CancellationToken ct)
    {
        return Ok(await _service.ListarPorContratoAsync(contratoId, ct));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(int contratoId, [FromBody] CrearRegistroDiarioRequest dto, CancellationToken ct)
    {
        var id = await _service.CrearAsync(contratoId, dto, ct);
        return CreatedAtAction(nameof(Listar), new { contratoId }, new { id });
    }
}
