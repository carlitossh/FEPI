namespace Fepi.Api.Models;
public class Garantia
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public TipoGarantia Tipo { get; set; }
    public decimal Monto { get; set; }
    public decimal Porcentaje { get; set; }
    public DateOnly Vigencia { get; set; }
    public EstadoGarantia Estado { get; set; } = EstadoGarantia.Vigente;
}