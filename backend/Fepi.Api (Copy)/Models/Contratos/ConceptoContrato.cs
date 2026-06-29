namespace Fepi.Api.Models;

public class ConceptoContrato
{
    public int Id { get; set; }

    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public int? SeccionConceptoId { get; set; }
    public SeccionConcepto? SeccionConcepto { get; set; }

    public string Clave { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string UnidadMedida { get; set; } = null!;
    public decimal CantidadContratada { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal PrecioTotal => CantidadContratada * PrecioUnitario;

    // Mantenido para compatibilidad con código existente
    public decimal Importe => PrecioTotal;

    public bool EsExtraordinario { get; set; } = false;
    public int? ConvenioModificatorioId { get; set; }
    public ConvenioModificatorio? ConvenioModificatorio { get; set; }
    public bool Activo { get; set; } = true;
}
