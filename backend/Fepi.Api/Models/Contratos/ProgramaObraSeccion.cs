namespace Fepi.Api.Models;

public class ProgramaObraSeccion
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public int SeccionConceptoId { get; set; }
    public SeccionConcepto? SeccionConcepto { get; set; }

    public int PeriodoInicioId { get; set; }
    public PeriodoContrato? PeriodoInicio { get; set; }

    public int PeriodoFinId { get; set; }
    public PeriodoContrato? PeriodoFin { get; set; }

    public ICollection<ProgramaObraPeriodo> Periodos { get; set; } = new List<ProgramaObraPeriodo>();
}
