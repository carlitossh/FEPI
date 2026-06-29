using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Fepi.Api.Services;

public class ArchivoService : IArchivoService
{
    private readonly FepiDbContext _context;
    private readonly string _carpetaBase;

    public ArchivoService(FepiDbContext context, IConfiguration configuration)
    {
        _context = context;
        _carpetaBase = configuration["Archivos:CarpetaBase"] ?? Path.Combine(Directory.GetCurrentDirectory(), "archivos");
    }

    public async Task<ArchivoResponse> SubirAsync(
        IFormFile archivo,
        int usuarioId,
        CancellationToken ct = default)
    {
        if (archivo.Length == 0)
            throw new InvalidOperationException("El archivo está vacío.");

        if (!Directory.Exists(_carpetaBase))
            Directory.CreateDirectory(_carpetaBase);

        var extension = Path.GetExtension(archivo.FileName);
        var nombreGuardado = $"{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(_carpetaBase, nombreGuardado);

        await using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            await archivo.CopyToAsync(stream, ct);

        var registro = new ArchivoEvidencia
        {
            NombreOriginal = archivo.FileName,
            NombreGuardado = nombreGuardado,
            RutaLocal = rutaCompleta,
            TipoContenido = archivo.ContentType,
            TamanoBytes = archivo.Length,
            FechaSubida = DateTime.UtcNow,
            UsuarioSubioId = usuarioId
        };

        _context.ArchivosEvidencia.Add(registro);
        await _context.SaveChangesAsync(ct);

        return MapearResponse(registro);
    }

    public async Task<ArchivoResponse> ObtenerMetadataAsync(int id, CancellationToken ct = default)
    {
        var archivo = await _context.ArchivosEvidencia.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Archivo no encontrado.");

        return MapearResponse(archivo);
    }

    public async Task<(byte[] Contenido, string TipoContenido, string NombreOriginal)> DescargarAsync(
        int id, CancellationToken ct = default)
    {
        var archivo = await _context.ArchivosEvidencia.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Archivo no encontrado.");

        if (!File.Exists(archivo.RutaLocal))
            throw new InvalidOperationException("El archivo no existe en el servidor.");

        var contenido = await File.ReadAllBytesAsync(archivo.RutaLocal, ct);
        return (contenido, archivo.TipoContenido, archivo.NombreOriginal);
    }

    public async Task<List<ArchivoResponse>> ListarTodosAsync(CancellationToken ct = default)
    {
        var archivos = await _context.ArchivosEvidencia
            .OrderByDescending(a => a.FechaSubida)
            .ToListAsync(ct);

        return archivos.Select(MapearResponse).ToList();
    }

    private static ArchivoResponse MapearResponse(ArchivoEvidencia a) =>
        new(a.Id, a.NombreOriginal, a.TipoContenido, a.TamanoBytes, a.FechaSubida, a.UsuarioSubioId);
}
