using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/empresas")]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _service;

    public EmpresasController(IEmpresaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmpresaResponse>>> Listar(CancellationToken ct)
    {
        return Ok(await _service.ListarAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmpresaResponse>> Obtener(int id, CancellationToken ct)
    {
        return Ok(await _service.ObtenerAsync(id, ct));
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEmpresaRequest dto, CancellationToken ct)
    {
        var id = await _service.CrearAsync(dto, ct);
        var empresa = await _service.ObtenerAsync(id, ct);
        return CreatedAtAction(nameof(Obtener), new { id }, empresa);
    }
}
