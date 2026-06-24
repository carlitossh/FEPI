using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IConvenioRepository : IGenericRepository<ConvenioModificatorio>
{
    Task<ConvenioModificatorio?> GetConDetalleAsync(int id, CancellationToken ct = default);
    Task<List<ConvenioModificatorio>> GetByContratoAsync(int contratoId, CancellationToken ct = default);
}