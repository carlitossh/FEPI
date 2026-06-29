namespace Fepi.Api.Models;
public class ConvenioResolucionDependencia
{
    public int Id { get; set; }
    public int ConvenioModificatorioId { get; set; }
    public ConvenioModificatorio? ConvenioModificatorio { get; set; }

    public bool Aprobado { get; set; }
    public string? MotivoRechazo { get; set; }
    public int UsuarioDependenciaId { get; set; }
    public Usuario? UsuarioDependencia { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
