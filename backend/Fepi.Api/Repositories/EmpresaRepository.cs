using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class EmpresaRepository : GenericRepository<Empresa>, IEmpresaRepository
{
    private readonly FepiDbContext _context;

    public EmpresaRepository(FepiDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Empresa>> GetConContratosAsync(CancellationToken ct = default)
        => await _context.Empresas
            .Include(e => e.Contratos)
            .Include(e => e.RepresentanteUsuario)
            .ToListAsync(ct);
}
