namespace Fepi.Api.Models;
public class Finiquito
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public decimal TotalPagado { get; set; }
    public decimal TotalPendiente { get; set; }
    public decimal TotalDeductivas { get; set; }
    public decimal TotalRetenciones { get; set; }
    public decimal MontoFinal => TotalPagado - TotalDeductivas - TotalRetenciones;

    public string? UrlReporteFiniquito { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
}