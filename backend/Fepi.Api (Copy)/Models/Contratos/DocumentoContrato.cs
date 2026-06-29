namespace Fepi.Api.Models;
public class DocumentoContrato
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public string Nombre { get; set; } = null!;
    public string TipoDocumento { get; set; } = null!;
    public string UrlArchivo { get; set; } = null!;
}