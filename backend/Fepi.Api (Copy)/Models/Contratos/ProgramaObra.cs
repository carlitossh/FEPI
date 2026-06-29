namespace Fepi.Api.Models;
public class ProgramaObraItem
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public string Periodo { get; set; } = null!;
    public decimal PorcentajeProgramado { get; set; }
    public decimal MontoProgramado { get; set; }
}