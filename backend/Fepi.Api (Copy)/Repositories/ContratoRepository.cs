using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class ContratoRepository : GenericRepository<Contrato>, IContratoRepository
{
    private readonly FepiDbContext _context;

    public ContratoRepository(FepiDbContext context) : base(context) => _context = context;

    public async Task<Contrato?> GetConCatalogoYGarantiasAsync(int id, CancellationToken ct = default)
    {
        return await _context.Contratos
            .Include(c => c.ConceptoContratos)
            .Include(c => c.Garantias)
            .Include(c => c.ProgramaObra)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<List<Contrato>> GetByDependenciaAsync(string dependencia, EstadoContrato? Estado, CancellationToken ct = default)
    {
        var query = _context.Contratos.AsNoTracking().Where(c => c.DependenciaContratante == dependencia);

        if (Estado.HasValue)
            query = query.Where(c => c.Estado == Estado.Value);

        return await query.ToListAsync(ct);
    }
}
