namespace Fepi.Api.Models;
public class EstimacionConcepto
{
    public int Id { get; set; }
    public int EstimacionId { get; set; }
    public Estimacion? Estimacion { get; set; }

    public int ConceptoContratoId { get; set; }
    public ConceptoContrato? ConceptoContrato { get; set; }

    public decimal CantidadEjecutada { get; set; }
    public decimal Importe { get; set; }
}
