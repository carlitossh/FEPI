using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IBitacoraNotaRepository : IGenericRepository<BitacoraNota>
{
    Task<BitacoraNota?> GetConFirmasAsync(int id, CancellationToken ct = default);
    Task<int> GetUltimoFolioAsync(int BitacoraId, CancellationToken ct = default);
    Task<List<BitacoraNota>> BuscarAsync(int BitacoraId, string? asunto, DateOnly? fechaInicio, DateOnly? fechaFin, int? actorId, CancellationToken ct = default);
}