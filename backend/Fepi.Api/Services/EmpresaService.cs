using System.Text.RegularExpressions;
using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class EmpresaService : IEmpresaService
{
    private readonly FepiDbContext _context;

    public EmpresaService(FepiDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmpresaResponse>> ListarAsync(CancellationToken ct = default)
    {
        var empresas = await _context.Empresas
            .Include(e => e.RepresentanteUsuario)
            .ToListAsync(ct);

        return empresas.Select(e => new EmpresaResponse(
            e.Id,
            e.Nombre,
            e.Rfc,
            e.RepresentanteUsuarioId,
            e.RepresentanteUsuario.Nombre
        )).ToList();
    }

    public async Task<EmpresaResponse> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var e = await _context.Empresas
            .Include(x => x.RepresentanteUsuario)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new InvalidOperationException("Empresa no encontrada.");

        return new EmpresaResponse(
            e.Id,
            e.Nombre,
            e.Rfc,
            e.RepresentanteUsuarioId,
            e.RepresentanteUsuario.Nombre
        );
    }

    public async Task<int> CrearAsync(CrearEmpresaRequest dto, CancellationToken ct = default)
    {
        var representante = await _context.Usuarios.FindAsync(new object[] { dto.RepresentanteUsuarioId }, ct)
            ?? throw new InvalidOperationException("Usuario representante no encontrado.");

        if (representante.Rol != RolUsuario.Superintendente)
            throw new InvalidOperationException("El representante debe tener rol Superintendente.");

        var rfc = NormalizarRfc(dto.Rfc);

        if (await _context.Empresas.AnyAsync(e => e.Rfc == rfc, ct))
            throw new InvalidOperationException("Ya existe una empresa con ese RFC.");

        var empresa = new Empresa
        {
            Nombre = dto.Nombre,
            Rfc = rfc,
            RepresentanteUsuarioId = dto.RepresentanteUsuarioId
        };

        _context.Empresas.Add(empresa);
        await _context.SaveChangesAsync(ct);

        return empresa.Id;
    }

    public async Task ActualizarAsync(int id, CrearEmpresaRequest dto, CancellationToken ct = default)
    {
        var empresa = await _context.Empresas.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Empresa no encontrada.");

        var representante = await _context.Usuarios.FindAsync(new object[] { dto.RepresentanteUsuarioId }, ct)
            ?? throw new InvalidOperationException("Usuario representante no encontrado.");

        if (representante.Rol != RolUsuario.Superintendente)
            throw new InvalidOperationException("El representante debe tener rol Superintendente.");

        var rfc = NormalizarRfc(dto.Rfc);

        if (await _context.Empresas.AnyAsync(e => e.Id != id && e.Rfc == rfc, ct))
            throw new InvalidOperationException("Ya existe otra empresa con ese RFC.");

        empresa.Nombre = dto.Nombre;
        empresa.Rfc = rfc;
        empresa.RepresentanteUsuarioId = dto.RepresentanteUsuarioId;

        await _context.SaveChangesAsync(ct);
    }

    public async Task ActualizarRepresentanteAsync(int empresaId, int usuarioId, CancellationToken ct = default)
    {
        var empresa = await _context.Empresas.FindAsync(new object[] { empresaId }, ct)
            ?? throw new InvalidOperationException("Empresa no encontrada.");

        var usuario = await _context.Usuarios.FindAsync(new object[] { usuarioId }, ct)
            ?? throw new InvalidOperationException("Usuario no encontrado.");

        if (usuario.Rol != RolUsuario.Superintendente)
            throw new InvalidOperationException("El representante debe tener rol Superintendente.");

        empresa.RepresentanteUsuarioId = usuarioId;

        await _context.SaveChangesAsync(ct);
    }

    private static string NormalizarRfc(string rfc)
    {
        var normalizado = (rfc ?? "").Trim().ToUpperInvariant();

        if (!Regex.IsMatch(normalizado, @"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$"))
            throw new InvalidOperationException("El RFC no tiene un formato válido.");

        return normalizado;
    }
}