namespace Fepi.Api.Models;

public class ConvenioRevisionSupervision
{
    public int Id { get; set; }
    public int ConvenioModificatorioId { get; set; }
    public ConvenioModificatorio? ConvenioModificatorio { get; set; }

    public DictamenTecnico Decision { get; set; }
    public string Justificacion { get; set; } = null!;
    public int SupervisorId { get; set; }
    public Usuario? Supervisor { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
