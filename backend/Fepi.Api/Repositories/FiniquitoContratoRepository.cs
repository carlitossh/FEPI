using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class FiniquitoContratoRepository : IFiniquitoContratoRepository
{
    private readonly FepiDbContext _context;

    public FiniquitoContratoRepository(FepiDbContext context) => _context = context;

    public async Task<FiniquitoContrato?> ObtenerPorContratoAsync(int contratoId, CancellationToken ct = default)
        => await _context.FiniquitosContrato
            .Include(f => f.BitacoraNotaCierre)
                .ThenInclude(n => n!.Firmas)
            .FirstOrDefaultAsync(f => f.ContratoId == contratoId, ct);

    public async Task<FiniquitoContrato?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        => await _context.FiniquitosContrato
            .Include(f => f.BitacoraNotaCierre)
                .ThenInclude(n => n!.Firmas)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task AddAsync(FiniquitoContrato finiquito, CancellationToken ct = default)
        => await _context.FiniquitosContrato.AddAsync(finiquito, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
