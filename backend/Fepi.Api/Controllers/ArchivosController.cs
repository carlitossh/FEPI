using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/archivos")]
public class ArchivosController : ControllerBase
{
    private readonly IArchivoService _service;

    public ArchivosController(IArchivoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ArchivoResponse>>> Listar(CancellationToken ct)
    {
        var archivos = await _service.ListarTodosAsync(ct);
        return Ok(archivos);
    }

    [HttpGet("{id:int}/metadata")]
    public async Task<ActionResult<ArchivoResponse>> ObtenerMetadata(int id, CancellationToken ct)
    {
        var archivo = await _service.ObtenerMetadataAsync(id, ct);
        return Ok(archivo);
    }

    [HttpGet("{id:int}/descargar")]
    public async Task<IActionResult> Descargar(int id, CancellationToken ct)
    {
        var (contenido, tipo, nombre) = await _service.DescargarAsync(id, ct);
        return File(contenido, tipo, nombre);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ArchivoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ArchivoResponse>> Subir(
        [FromForm] SubirArchivoRequest request,
        CancellationToken ct)
    {
        var resultado = await _service.SubirAsync(request.Archivo, request.UsuarioId, ct);
        return CreatedAtAction(nameof(ObtenerMetadata), new { id = resultado.Id }, resultado);
    }
}
