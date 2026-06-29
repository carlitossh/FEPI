namespace Fepi.Api.Models;

public class ConvenioDocumento
{
    public int Id { get; set; }
    public int ConvenioModificatorioId { get; set; }
    public ConvenioModificatorio? ConvenioModificatorio { get; set; }

    public string Nombre { get; set; } = null!;
    public string UrlArchivo { get; set; } = null!;
}
