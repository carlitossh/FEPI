using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IArchivoEvidenciaRepository : IGenericRepository<ArchivoEvidencia>
{
    Task<List<ArchivoEvidencia>> GetTodosAsync(CancellationToken ct = default);
}
