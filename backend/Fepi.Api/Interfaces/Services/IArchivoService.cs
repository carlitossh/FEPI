using Fepi.Api.DTOs;
using Fepi.Api.Models;
using Microsoft.AspNetCore.Http;

namespace Fepi.Api.Interfaces;

public interface IArchivoService
{
    Task<ArchivoResponse> SubirAsync(IFormFile archivo, EntidadArchivo entidad, int entidadId, int usuarioId, CancellationToken ct = default);
    Task<ArchivoResponse> ObtenerMetadataAsync(int id, CancellationToken ct = default);
    Task<(byte[] Contenido, string TipoContenido, string NombreOriginal)> DescargarAsync(int id, CancellationToken ct = default);
    Task<List<ArchivoResponse>> ListarPorEntidadAsync(EntidadArchivo entidad, int entidadId, CancellationToken ct = default);
}
