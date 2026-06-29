namespace Fepi.Api.Models;
public class EstimacionConcepto
{
    public int Id { get; set; }
    public int EstimacionId { get; set; }
    public Estimacion? Estimacion { get; set; }

    public int ConceptoContratoId { get; set; }
    public ConceptoContrato? ConceptoContrato { get; set; }

    public decimal CantidadEjecutadaPeriodo { get; set; }
    public decimal PrecioUnitarioActual { get; set; }
    public decimal ImporteTotal { get; set; }

    // Snapshots calculados por el servicio
    public decimal CantidadAcumuladaAnterior { get; set; }
    public decimal CantidadAcumuladaActual { get; set; }
    public decimal CantidadPorEjecutar { get; set; }
}
