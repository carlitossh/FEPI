using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class RegistroDiarioRepository : GenericRepository<RegistroDiario>, IRegistroDiarioRepository
{
    private readonly FepiDbContext _context;

    public RegistroDiarioRepository(FepiDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<RegistroDiario>> GetByContratoAsync(int contratoId, CancellationToken ct = default)
        => await _context.RegistrosDiarios
            .Where(r => r.ContratoId == contratoId)
            .Include(r => r.ResponsableUsuario)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync(ct);
}
