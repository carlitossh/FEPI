namespace Fepi.Api.Models;

public class Estimacion
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public int NumeroEstimacion { get; set; }

    // Mantenido para compatibilidad
    public int NumeroCorrelativo
    {
        get => NumeroEstimacion;
        set => NumeroEstimacion = value;
    }

    public int? PeriodoContratoId { get; set; }
    public PeriodoContrato? PeriodoContrato { get; set; }

    // Etiqueta de periodo para compatibilidad (ej. "2026-03")
    public string Periodo { get; set; } = null!;

    public EstadoEstimacion Estado { get; set; } = EstadoEstimacion.Borrador;
    public EstadoPagoEstimacion EstadoPago { get; set; } = EstadoPagoEstimacion.SinPago;
    public decimal MontoPagadoAcumulado { get; set; } = 0;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEnvio { get; set; }
    public int? UsuarioEnvioId { get; set; }
    public Usuario? UsuarioEnvio { get; set; }

    public DateTime? FechaAprobacionSupervision { get; set; }
    public int? UsuarioAprobacionSupervisionId { get; set; }
    public Usuario? UsuarioAprobacionSupervision { get; set; }

    public DateTime? FechaAprobacionResidencia { get; set; }
    public int? UsuarioAprobacionResidenciaId { get; set; }
    public Usuario? UsuarioAprobacionResidencia { get; set; }

    public DateTime? FechaPago { get; set; }
    public int? ArchivoComprobantePagoId { get; set; }
    public ArchivoEvidencia? ArchivoComprobantePago { get; set; }

    public ICollection<EstimacionConcepto> Conceptos { get; set; } = new List<EstimacionConcepto>();
    public ICollection<EstimacionDocumento> Documentos { get; set; } = new List<EstimacionDocumento>();
    public ICollection<EstimacionNotaBitacora> NotasVinculadas { get; set; } = new List<EstimacionNotaBitacora>();
    public ICollection<EstimacionObservacion> Observaciones { get; set; } = new List<EstimacionObservacion>();
    public ICollection<EstimacionHistorial> Historial { get; set; } = new List<EstimacionHistorial>();
    public ICollection<EstimacionPago> Pagos { get; set; } = new List<EstimacionPago>();
}
