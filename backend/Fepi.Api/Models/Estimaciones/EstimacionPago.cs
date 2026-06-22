namespace Fepi.Api.Models;
public class EstimacionPago
{
    public int Id { get; set; }
    public int EstimacionId { get; set; }
    public Estimacion? Estimacion { get; set; }

    public DateOnly FechaPago { get; set; }
    public string ReferenciaBancaria { get; set; } = null!;
    public decimal MontoPagado { get; set; }
}
