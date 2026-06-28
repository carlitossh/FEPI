using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class AlertaUsuarioRepository : IAlertaUsuarioRepository
{
    private readonly FepiDbContext _context;

    public AlertaUsuarioRepository(FepiDbContext context) => _context = context;

    public async Task<List<AlertaUsuario>> ObtenerPorUsuarioAsync(int usuarioId, CancellationToken ct = default)
        => await _context.AlertasUsuario
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.FechaCreacion)
            .ToListAsync(ct);

    public async Task<List<AlertaUsuario>> ObtenerNoLeidasPorUsuarioAsync(int usuarioId, CancellationToken ct = default)
        => await _context.AlertasUsuario
            .Where(a => a.UsuarioId == usuarioId && !a.Leida)
            .OrderByDescending(a => a.FechaCreacion)
            .ToListAsync(ct);

    public async Task<AlertaUsuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        => await _context.AlertasUsuario.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(AlertaUsuario alerta, CancellationToken ct = default)
        => await _context.AlertasUsuario.AddAsync(alerta, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
