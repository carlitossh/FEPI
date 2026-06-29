using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IRegistroDiarioRepository : IGenericRepository<RegistroDiario>
{
    Task<List<RegistroDiario>> GetByContratoAsync(int contratoId, CancellationToken ct = default);
}
