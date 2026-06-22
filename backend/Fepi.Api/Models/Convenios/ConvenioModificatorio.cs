namespace Fepi.Api.Models;

public class ConvenioModificatorio
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public TipoModificacionConvenio Tipo { get; set; }
    public string Justificacion { get; set; } = null!;
    public EstadoConvenio Estado { get; set; } = EstadoConvenio.Solicitada;

    public decimal? MontoSolicitado { get; set; }
    public int? PlazoDiasSolicitado { get; set; }
    public decimal VariacionAcumuladaPorcentaje { get; set; }

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public int SolicitanteId { get; set; }
    public Usuario? Solicitante { get; set; }
    public ICollection<ConvenioDocumento> Documentos { get; set; } = new List<ConvenioDocumento>();
    public ConvenioRevisionSupervision? RevisionSupervision { get; set; }
    public ConvenioPromocionResidencia? PromocionResidencia { get; set; }
    public ConvenioResolucionDependencia? ResolucionDependencia { get; set; }
}