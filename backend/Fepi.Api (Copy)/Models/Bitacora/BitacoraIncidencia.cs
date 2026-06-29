namespace Fepi.Api.Models;
public class BitacoraIncidencia
{
    public int Id { get; set; }
    public int BitacoraId { get; set; }
    public Bitacora? Bitacora { get; set; }

    public DateOnly FechaEvento { get; set; }
    public string Descripcion { get; set; } = null!;
    public string UrlFotografia { get; set; } = null!;
    public int ActorRegistroId { get; set; }
    public Usuario? ActorRegistro { get; set; }

    public int? NotaGeneradaId { get; set; }
    public BitacoraNota? NotaGenerada { get; set; }
}