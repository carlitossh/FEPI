using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IArchivoEvidenciaRepository : IGenericRepository<ArchivoEvidencia>
{
    Task<List<ArchivoEvidencia>> GetByEntidadAsync(EntidadArchivo entidad, int entidadId, CancellationToken ct = default);
}
