using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class UsuarioService : IUsuarioService
{
    private readonly FepiDbContext _context;

    public UsuarioService(FepiDbContext context) => _context = context;

    public async Task<List<UsuarioDto>> ListarAsync(CancellationToken ct = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioDto(u.Id, u.Nombre, u.Correo, u.Rol, u.Activo))
            .ToListAsync(ct);
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int usuarioId, CancellationToken ct = default)
    {
        var u = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == usuarioId, ct);
        return u is null ? null : new UsuarioDto(u.Id, u.Nombre, u.Correo, u.Rol, u.Activo);
    }

    public async Task<UsuarioDto> CrearAsync(CrearUsuarioRequest dto, CancellationToken ct = default)
    {
        var existe = await _context.Usuarios
            .AnyAsync(u => u.Username == dto.Username || u.Correo == dto.Correo, ct);
        if (existe)
            throw new InvalidOperationException("Ya existe un usuario con ese username o correo.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            Username = dto.Username,
            Password = dto.Password,
            Rol = dto.Rol,
            Activo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(ct);

        return new UsuarioDto(usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol, usuario.Activo);
    }

    public async Task<UsuarioLoginResponse> LoginAsync(UsuarioLoginRequest dto, CancellationToken ct = default)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.Password == dto.Password && u.Activo, ct)
            ?? throw new InvalidOperationException("Credenciales inválidas o usuario inactivo.");

        return new UsuarioLoginResponse(usuario.Id, usuario.Nombre, usuario.Username, usuario.Rol);
    }

    public async Task<List<UsuarioContratoDto>> ObtenerRolesYContratosAsync(int usuarioId, CancellationToken ct = default)
    {
        return await _context.UsuarioContratos
            .AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId)
            .Select(x => new UsuarioContratoDto(x.ContratoId, x.Rol))
            .ToListAsync(ct);
    }

    public async Task<List<UsuarioContratoDetalleDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        return await _context.UsuarioContratos
            .AsNoTracking()
            .Include(uc => uc.Usuario)
            .Where(uc => uc.ContratoId == contratoId)
            .Select(uc => new UsuarioContratoDetalleDto(
                uc.UsuarioId,
                uc.Usuario!.Nombre,
                uc.Usuario!.Correo,
                uc.Rol,
                uc.Usuario!.Activo))
            .ToListAsync(ct);
    }

    public async Task<UsuarioContratoDetalleDto> InvitarAsync(int contratoId, InvitarUsuarioContratoDto dto, CancellationToken ct = default)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == dto.Correo, ct);

        if (usuario is null)
        {
            usuario = new Usuario { Nombre = dto.Nombre, Correo = dto.Correo, Activo = true };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(ct);
        }

        var existe = await _context.UsuarioContratos
            .AnyAsync(uc => uc.UsuarioId == usuario.Id && uc.ContratoId == contratoId, ct);

        if (!existe)
        {
            _context.UsuarioContratos.Add(new UsuarioContrato
            {
                UsuarioId = usuario.Id,
                ContratoId = contratoId,
                Rol = dto.Rol
            });
            await _context.SaveChangesAsync(ct);
        }

        return new UsuarioContratoDetalleDto(usuario.Id, usuario.Nombre, usuario.Correo, dto.Rol, usuario.Activo);
    }

    public async Task ActualizarRolAsync(int usuarioId, ActualizarRolDto dto, CancellationToken ct = default)
    {
        var uc = await _context.UsuarioContratos
            .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.ContratoId == dto.ContratoId, ct)
            ?? throw new InvalidOperationException("Relación usuario-contrato no encontrada.");

        uc.Rol = dto.Rol;
        await _context.SaveChangesAsync(ct);
    }

    public async Task SuspenderAsync(int usuarioId, CancellationToken ct = default)
    {
        var usuario = await _context.Usuarios.FindAsync([usuarioId], ct)
            ?? throw new InvalidOperationException("Usuario no encontrado.");
        usuario.Activo = false;
        await _context.SaveChangesAsync(ct);
    }

    public async Task ActivarAsync(int usuarioId, CancellationToken ct = default)
    {
        var usuario = await _context.Usuarios.FindAsync([usuarioId], ct)
            ?? throw new InvalidOperationException("Usuario no encontrado.");
        usuario.Activo = true;
        await _context.SaveChangesAsync(ct);
    }
}
