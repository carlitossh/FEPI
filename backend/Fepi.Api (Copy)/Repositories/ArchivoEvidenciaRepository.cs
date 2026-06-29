using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class ArchivoEvidenciaRepository : GenericRepository<ArchivoEvidencia>, IArchivoEvidenciaRepository
{
    private readonly FepiDbContext _context;

    public ArchivoEvidenciaRepository(FepiDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ArchivoEvidencia>> GetTodosAsync(CancellationToken ct = default)
        => await _context.ArchivosEvidencia
            .OrderByDescending(a => a.FechaSubida)
            .ToListAsync(ct);
}
