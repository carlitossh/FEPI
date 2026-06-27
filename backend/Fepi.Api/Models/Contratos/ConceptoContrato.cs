namespace Fepi.Api.Models;

public class ConceptoContrato
{
    public int Id { get; set; }

    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public string Clave { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string UnidadMedida { get; set; } = null!;
    public decimal CantidadContratada { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal Importe => CantidadContratada * PrecioUnitario;
}