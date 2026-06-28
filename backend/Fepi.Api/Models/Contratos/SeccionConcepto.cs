namespace Fepi.Api.Models;

public class SeccionConcepto
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<ConceptoContrato> Conceptos { get; set; } = new List<ConceptoContrato>();
}
