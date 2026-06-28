namespace Fepi.Api.Models;

public class ProgramaObraPeriodo
{
    public int Id { get; set; }
    public int ProgramaObraSeccionId { get; set; }
    public ProgramaObraSeccion? ProgramaObraSeccion { get; set; }

    public int PeriodoContratoId { get; set; }
    public PeriodoContrato? PeriodoContrato { get; set; }

    public decimal PorcentajePlanificadoPeriodo { get; set; }
    public decimal ImportePlanificadoPeriodo { get; set; }
    public decimal AvanceParcialPlanificado { get; set; }
}
