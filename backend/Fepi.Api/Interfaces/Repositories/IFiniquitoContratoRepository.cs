using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IFiniquitoContratoRepository
{
    Task<FiniquitoContrato?> ObtenerPorContratoAsync(int contratoId, CancellationToken ct = default);
    Task<FiniquitoContrato?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(FiniquitoContrato finiquito, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
