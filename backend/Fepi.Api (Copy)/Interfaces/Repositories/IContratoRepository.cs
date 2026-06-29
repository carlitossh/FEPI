using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IContratoRepository : IGenericRepository<Contrato>
{
    Task<Contrato?> GetConCatalogoYGarantiasAsync(int id, CancellationToken ct = default);
    Task<List<Contrato>> GetByDependenciaAsync(string dependencia, EstadoContrato? Estado, CancellationToken ct = default);
}