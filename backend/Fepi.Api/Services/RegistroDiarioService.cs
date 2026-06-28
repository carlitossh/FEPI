using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class RegistroDiarioService : IRegistroDiarioService
{
    private readonly FepiDbContext _context;

    public RegistroDiarioService(FepiDbContext context)
    {
        _context = context;
    }

    public async Task<List<RegistroDiarioResponse>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var registros = await _context.RegistrosDiarios
            .Where(r => r.ContratoId == contratoId)
            .Include(r => r.ResponsableUsuario)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync(ct);

        return registros.Select(r => new RegistroDiarioResponse(
            r.Id, r.ContratoId, r.Fecha, r.ResponsableUsuarioId,
            r.ResponsableUsuario?.Nombre ?? "", r.Descripcion, r.FechaRegistro)).ToList();
    }

    public async Task<int> CrearAsync(int contratoId, CrearRegistroDiarioRequest dto, CancellationToken ct = default)
    {
        var contratoExiste = await _context.Contratos.AnyAsync(c => c.Id == contratoId, ct);
        if (!contratoExiste)
            throw new InvalidOperationException("Contrato no encontrado.");

        var responsableExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.ResponsableUsuarioId, ct);
        if (!responsableExiste)
            throw new InvalidOperationException("Usuario responsable no encontrado.");

        var registro = new RegistroDiario
        {
            ContratoId = contratoId,
            Fecha = dto.Fecha,
            ResponsableUsuarioId = dto.ResponsableUsuarioId,
            Descripcion = dto.Descripcion,
            FechaRegistro = DateTime.UtcNow
        };

        _context.RegistrosDiarios.Add(registro);
        await _context.SaveChangesAsync(ct);
        return registro.Id;
    }
}
