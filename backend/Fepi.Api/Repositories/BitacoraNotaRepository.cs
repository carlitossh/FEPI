using Fepi.Api.Data;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Repositories;

public class BitacoraNotaRepository : GenericRepository<BitacoraNota>, IBitacoraNotaRepository
{
    private readonly FepiDbContext _context;

    public BitacoraNotaRepository(FepiDbContext context) : base(context) => _context = context;

    public async Task<BitacoraNota?> GetConFirmasAsync(int id, CancellationToken ct = default)
    {
        return await _context.BitacoraNotas.Include(n => n.Firmas).FirstOrDefaultAsync(n => n.Id == id, ct);
    }

    public async Task<int> GetUltimoFolioAsync(int BitacoraId, CancellationToken ct = default)
    {
        return await _context.BitacoraNotas
            .Where(n => n.BitacoraId == BitacoraId)
            .Select(n => n.Folio)
            .DefaultIfEmpty(0)
            .MaxAsync(ct);
    }

    public async Task<List<BitacoraNota>> BuscarAsync(int BitacoraId, string? asunto, DateOnly? fechaInicio, DateOnly? fechaFin, int? actorId, CancellationToken ct = default)
    {
        var query = _context.BitacoraNotas.Include(n => n.Firmas)
            .Where(n => n.BitacoraId == BitacoraId);

        if (!string.IsNullOrWhiteSpace(asunto))
            query = query.Where(n => n.Asunto.Contains(asunto));

        if (fechaInicio.HasValue)
            query = query.Where(n => n.FechaRegistro >= fechaInicio.Value.ToDateTime(TimeOnly.MinValue));

        if (fechaFin.HasValue)
            query = query.Where(n => n.FechaRegistro <= fechaFin.Value.ToDateTime(TimeOnly.MaxValue));

        if (actorId.HasValue)
            query = query.Where(n => n.Firmas.Any(f => f.EsEmisor && f.UsuarioId == actorId.Value));

        return await query.OrderByDescending(n => n.FechaRegistro).ToListAsync(ct);
    }
}
