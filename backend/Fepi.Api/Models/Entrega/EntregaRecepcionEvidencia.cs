namespace Fepi.Api.Models;
public class EntregaRecepcionEvidencia
{
    public int Id { get; set; }
    public int EntregaRecepcionId { get; set; }
    public EntregaRecepcion? EntregaRecepcion { get; set; }

    public string UrlArchivo { get; set; } = null!;
}