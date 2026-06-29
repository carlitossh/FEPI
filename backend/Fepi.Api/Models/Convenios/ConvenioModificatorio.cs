namespace Fepi.Api.Models;

public class ConvenioModificatorio
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public string NumeroConvenio { get; set; } = null!;
    public TipoConvenio Tipo { get; set; }
    public string Descripcion { get; set; } = null!;
    public string Justificacion { get; set; } = null!;
    public string? Observaciones { get; set; }
    public EstadoConvenio Estado { get; set; } = EstadoConvenio.Revisado;

    public decimal? MontoSolicitado { get; set; }
    public int? PlazoDiasSolicitado { get; set; }
    public decimal VariacionAcumuladaPorcentaje { get; set; }

    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public DateTime? FechaAutorizacion { get; set; }

    public int SolicitanteId { get; set; }
    public Usuario? Solicitante { get; set; }

    public bool Aplicado { get; set; } = false;
    public DateTime? FechaAplicacion { get; set; }

    public ICollection<ConvenioDocumento> Documentos { get; set; } = new List<ConvenioDocumento>();
    public ICollection<ConvenioCambio> Cambios { get; set; } = new List<ConvenioCambio>();
    public ICollection<ConvenioHistorial> Historial { get; set; } = new List<ConvenioHistorial>();
    public ConvenioRevisionSupervision? RevisionSupervision { get; set; }
    public ConvenioPromocionResidencia? PromocionResidencia { get; set; }
    public ConvenioResolucionDependencia? ResolucionDependencia { get; set; }
}
