namespace Fepi.Api.Models;

public class ArchivoEvidencia
{
    public int Id { get; set; }

    public string NombreOriginal { get; set; } = null!;
    public string NombreGuardado { get; set; } = null!;
    public string RutaLocal { get; set; } = null!;
    public string TipoContenido { get; set; } = null!;
    public long TamanoBytes { get; set; }

    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    public int UsuarioSubioId { get; set; }
    public Usuario? UsuarioSubio { get; set; }

    public EntidadArchivo EntidadRelacionada { get; set; }
    public int EntidadId { get; set; }
}
