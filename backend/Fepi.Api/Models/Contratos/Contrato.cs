namespace Fepi.Api.Models;
public class Contrato
{
    public int Id { get; set; }
    public string NumeroContrato { get; set; } = null!;
    public string? NombreObra { get; set; }
    public TipoContrato Tipo { get; set; }
    public decimal MontoContratado { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaTermino { get; set; }
    public PeriodoEstimacion PeriodoEstimacion { get; set; }
    public string DependenciaContratante { get; set; } = null!;
    public string ContratistaEmpresa { get; set; } = null!;
    public string ContratistaRepresentante { get; set; } = null!;
    public string? ResidenteNombre { get; set; }
    public string? SupervisorExternoNombre { get; set; }
    public string? SuperintendenteNombre { get; set; }
    public EstadoContrato Estado { get; set; } = EstadoContrato.Activo;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public ICollection<ConceptoContrato> ConceptoContratos { get; set; } = new List<ConceptoContrato>();
    public ICollection<ProgramaObraItem> ProgramaObra { get; set; } = new List<ProgramaObraItem>();
    public ICollection<Garantia> Garantias { get; set; } = new List<Garantia>();
    public ICollection<DocumentoContrato> Documentos { get; set; } = new List<DocumentoContrato>();
    public ICollection<UsuarioContrato> Usuarios { get; set; } = new List<UsuarioContrato>();
}