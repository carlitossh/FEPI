using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class EstimacionRepository : GenericRepository<Estimacion>, IEstimacionRepository
{
    private readonly FepiDbContext _context;

    public EstimacionRepository(FepiDbContext context) : base(context) => _context = context;

    public async Task<Estimacion?> GetConDetalleAsync(int id, CancellationToken ct = default)
    {
        return await _context.Estimaciones
            .Include(e => e.Conceptos).ThenInclude(c => c.ConceptoContrato)
            .Include(e => e.NotasVinculadas).ThenInclude(n => n.NotaBitacora)
            .Include(e => e.Observaciones)
            .Include(e => e.Contrato)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<List<Estimacion>> GetByContratoAsync(int contratoId, CancellationToken ct = default)
    {
        return await _context.Estimaciones
            .Include(e => e.Conceptos)
            .Where(e => e.ContratoId == contratoId)
            .OrderByDescending(e => e.NumeroCorrelativo)
            .ToListAsync(ct);
    }

    public async Task<int> GetUltimoCorrelativoAsync(int contratoId, CancellationToken ct = default)
    {
        return await _context.Estimaciones
            .Where(e => e.ContratoId == contratoId)
            .Select(e => e.NumeroCorrelativo)
            .DefaultIfEmpty(0)
            .MaxAsync(ct);
    }
}
