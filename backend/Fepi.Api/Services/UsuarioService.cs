using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class UsuarioService : IUsuarioService
{
    private readonly FepiDbContext _context;

    public UsuarioService(FepiDbContext context) => _context = context;

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int usuarioId, CancellationToken ct = default)
    {
        var u = await _context.Usuarios.FindAsync(new object[] { usuarioId }, ct);
        return u is null ? null : new UsuarioDto(u.Id, u.Nombre, u.Correo, u.Activo);
    }

    public async Task<List<UsuarioContratoDto>> ObtenerRolesYContratosAsync(int usuarioId, CancellationToken ct = default)
    {
        return await _context.UsuarioContratoes
            .Where(ucr => ucr.UsuarioId == usuarioId)
            .Select(ucr => new UsuarioContratoDto(ucr.ContratoId, ucr.Rol))
            .ToListAsync(ct);
    }
}
