namespace Fepi.Api.Models;

public class Estimacion
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public int NumeroCorrelativo { get; set; }
    public string Periodo { get; set; } = null!;
    public EstadoEstimacion Estado { get; set; } = EstadoEstimacion.Borrador;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEnvio { get; set; }
    public int? UsuarioEnvioId { get; set; }
    public Usuario? UsuarioEnvio { get; set; }

    public ICollection<EstimacionConcepto> Conceptos { get; set; } = new List<EstimacionConcepto>();
    public ICollection<EstimacionDocumento> Documentos { get; set; } = new List<EstimacionDocumento>();
    public ICollection<EstimacionNotaBitacora> NotasVinculadas { get; set; } = new List<EstimacionNotaBitacora>();
    public ICollection<EstimacionObservacion> Observaciones { get; set; } = new List<EstimacionObservacion>();
    public ICollection<EstimacionHistorial> Historial { get; set; } = new List<EstimacionHistorial>();
    public EstimacionPago? Pago { get; set; }
}