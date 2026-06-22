using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IEstimacionRepository : IGenericRepository<Estimacion>
{
    Task<Estimacion?> GetConDetalleAsync(int id, CancellationToken ct = default);
    Task<List<Estimacion>> GetByContratoAsync(int contratoId, CancellationToken ct = default);
    Task<int> GetUltimoCorrelativoAsync(int contratoId, CancellationToken ct = default);
}

public interface IBitacoraNotaRepository : IGenericRepository<BitacoraNota>
{
    Task<BitacoraNota?> GetConFirmasAsync(int id, CancellationToken ct = default);
    Task<int> GetUltimoFolioAsync(int BitacoraId, CancellationToken ct = default);
    Task<List<BitacoraNota>> BuscarAsync(int BitacoraId, string? asunto, DateOnly? fechaInicio, DateOnly? fechaFin, int? actorId, CancellationToken ct = default);
}

public interface IContratoRepository : IGenericRepository<Contrato>
{
    Task<Contrato?> GetConCatalogoYGarantiasAsync(int id, CancellationToken ct = default);
    Task<List<Contrato>> GetByDependenciaAsync(string dependencia, EstadoContrato? Estado, CancellationToken ct = default);
}

public interface IConvenioRepository : IGenericRepository<ConvenioModificatorio>
{
    Task<ConvenioModificatorio?> GetConDetalleAsync(int id, CancellationToken ct = default);
    Task<List<ConvenioModificatorio>> GetByContratoAsync(int contratoId, CancellationToken ct = default);
}
