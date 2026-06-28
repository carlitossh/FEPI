using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class ConvenioRepository : GenericRepository<ConvenioModificatorio>, IConvenioRepository
{
    private readonly FepiDbContext _context;

    public ConvenioRepository(FepiDbContext context) : base(context) => _context = context;

    public async Task<ConvenioModificatorio?> GetConDetalleAsync(int id, CancellationToken ct = default)
    {
        return await _context.ConveniosModificatorios
            .Include(c => c.RevisionSupervision)
            .Include(c => c.PromocionResidencia)
            .Include(c => c.ResolucionDependencia)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<List<ConvenioModificatorio>> GetByContratoAsync(int contratoId, CancellationToken ct = default)
    {
        return await _context.ConveniosModificatorios
            .Where(c => c.ContratoId == contratoId)
            .OrderByDescending(c => c.FechaEmision)
            .ToListAsync(ct);
    }
}
