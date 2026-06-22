namespace Fepi.Api.Models;

public class EntregaRecepcion
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public DateOnly FechaEntrega { get; set; }
    public string EstadoObraDescripcion { get; set; } = null!;
    public string EstadoGarantiasDescripcion { get; set; } = null!;

    public ICollection<EntregaRecepcionEvidencia> Evidencias { get; set; } = new List<EntregaRecepcionEvidencia>();
}