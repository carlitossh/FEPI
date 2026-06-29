using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fepi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _service;

    public AuthController(IUsuarioService service) => _service = service;

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UsuarioLoginResponse>>> Login(
        [FromBody] UsuarioLoginRequest dto,
        CancellationToken ct)
    {
        var result = await _service.LoginAsync(dto, ct);
        return Ok(new ApiResponse<UsuarioLoginResponse>(true, "Login exitoso.", result));
    }
}
