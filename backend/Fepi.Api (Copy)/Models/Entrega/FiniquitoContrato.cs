namespace Fepi.Api.Models;

public class FiniquitoContrato
{
    public int Id { get; set; }

    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public int? BitacoraNotaCierreId { get; set; }
    public BitacoraNota? BitacoraNotaCierre { get; set; }

    public DateTime FechaInicioCierre { get; set; }
    public DateTime? FechaFiniquito { get; set; }

    public decimal ImporteContratoOriginal { get; set; }
    public decimal ImporteConvenios { get; set; }
    public decimal ImporteContratoFinal { get; set; }
    public decimal ImporteEstimadoTotal { get; set; }
    public decimal ImportePagadoTotal { get; set; }
    public decimal SaldoPendiente { get; set; }

    public decimal Deductivas { get; set; }
    public decimal Retenciones { get; set; }
    public decimal PenasConvencionales { get; set; }
    public decimal ImporteFinalAFiniquitar { get; set; }

    public string? Observaciones { get; set; }
    public EstadoFiniquitoContrato Estado { get; set; } = EstadoFiniquitoContrato.PendienteNotaCierre;

    public int CreadoPorUsuarioId { get; set; }
    public Usuario? CreadoPor { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
