namespace Fepi.Api.Models;

public class AvanceDiario
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public DateOnly Fecha { get; set; }
    public int ConceptoContratoId { get; set; }
    public ConceptoContrato? ConceptoContrato { get; set; }

    public decimal CantidadEjecutada { get; set; }
    public string DescripcionActividad { get; set; } = null!;
    public int ActorId { get; set; }
    public Usuario? Actor { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<AvanceEvidencia> Evidencias { get; set; } = new List<AvanceEvidencia>();
}