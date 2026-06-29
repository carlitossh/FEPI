using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IEmpresaRepository : IGenericRepository<Empresa>
{
    Task<List<Empresa>> GetConContratosAsync(CancellationToken ct = default);
}
