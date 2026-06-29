using Fepi.Api.DTOs;
using Microsoft.AspNetCore.Http;

namespace Fepi.Api.Interfaces;

public interface IArchivoService
{
    Task<ArchivoResponse> SubirAsync(IFormFile archivo, int usuarioId, CancellationToken ct = default);
    Task<ArchivoResponse> ObtenerMetadataAsync(int id, CancellationToken ct = default);
    Task<(byte[] Contenido, string TipoContenido, string NombreOriginal)> DescargarAsync(int id, CancellationToken ct = default);
    Task<List<ArchivoResponse>> ListarTodosAsync(CancellationToken ct = default);
}
