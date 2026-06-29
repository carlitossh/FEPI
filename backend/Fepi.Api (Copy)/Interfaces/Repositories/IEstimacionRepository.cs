using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IEstimacionRepository : IGenericRepository<Estimacion>
{
    Task<Estimacion?> GetConDetalleAsync(int id, CancellationToken ct = default);
    Task<List<Estimacion>> GetByContratoAsync(int contratoId, CancellationToken ct = default);
    Task<int> GetUltimoCorrelativoAsync(int contratoId, CancellationToken ct = default);
}