namespace Fepi.Api.Models;

public class EstimacionDocumento
{
    public int Id { get; set; }
    public int EstimacionId { get; set; }
    public Estimacion? Estimacion { get; set; }

    public string Nombre { get; set; } = null!;
    public string TipoDocumento { get; set; } = null!;
    public string UrlArchivo { get; set; } = null!;
}