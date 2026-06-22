using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

/// <summary>SV-06 — Alertas y Notificaciones. Cubre HU-21. Solo panel, sin correo.</summary>
[ApiController]
[Route("api/alertas")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _service;
    public AlertasController(IAlertaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<AlertaDto>>> ObtenerActivasPorRol([FromQuery] RolSistema rol, [FromQuery] int? contratoId, CancellationToken ct)
        => Ok(await _service.ObtenerActivasPorRolAsync(rol, contratoId, ct));
}

/// <summary>SV-07 — Expediente Digital y Registro de Contrato. Cubre HU-22 a HU-24.</summary>
[ApiController]
[Route("api/contratos")]
public class ContratosController : ControllerBase
{
    private readonly IContratoService _service;
    public ContratosController(IContratoService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<int>> Crear([FromBody] CrearContratoDto dto, CancellationToken ct)
    {
        var id = await _service.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id }, id);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContratoDetalleDto>> ObtenerDetalle(int id, CancellationToken ct)
        => Ok(await _service.ObtenerDetalleAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<List<ContratoResumenDto>>> ListarPorDependencia([FromQuery] string dependencia, [FromQuery] EstadoContrato? Estado, CancellationToken ct)
        => Ok(await _service.ListarPorDependenciaAsync(dependencia, Estado, ct));

    [HttpPut("{id:int}/programa-obra")]
    public async Task<IActionResult> ActualizarProgramaObra(int id, [FromBody] ActualizarProgramaObraDto dto, CancellationToken ct)
    {
        await _service.ActualizarProgramaObraAsync(id, dto, ct);
        return NoContent();
    }
}

/// <summary>SV-08 — solo lectura; alta de usuarios y roles vía scripts (ver /Scripts).</summary>
[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;
    public UsuariosController(IUsuarioService service) => _service = service;

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var usuario = await _service.ObtenerPorIdAsync(id, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpGet("{id:int}/roles")]
    public async Task<ActionResult<List<UsuarioContratoDto>>> ObtenerRoles(int id, CancellationToken ct)
        => Ok(await _service.ObtenerRolesYContratosAsync(id, ct));
}

/// <summary>SV-10 — Panel Ejecutivo (Dashboard). Cubre HU-26.</summary>
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet("contrato/{contratoId:int}")]
    public async Task<ActionResult<DashboardContratoDto>> ObtenerPorContrato(int contratoId, CancellationToken ct)
        => Ok(await _service.ObtenerPorContratoAsync(contratoId, ct));
}
