namespace Fepi.Api.Models;
public class Contrato
{
    public int Id { get; set; }
    public string NumeroContrato { get; set; } = null!;
    public string? NumeroLicitacion { get; set; }
    public string? NombreObra { get; set; }
    public TipoContrato Tipo { get; set; }

    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    public string DependenciaContratante { get; set; } = null!;
    public string? UbicacionExacta { get; set; }

    public decimal ImporteTotal { get; set; }
    public decimal ImporteSinIVA { get; set; }
    public decimal IvaPorcentaje { get; set; } = 16m;
    public decimal IVA { get; set; }

    public ModalidadPago ModalidadPago { get; set; }
    public decimal PorcentajeAnticipo { get; set; }
    public decimal MontoAnticipo { get; set; }

    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaTermino { get; set; }

    public TipoPeriodoEstimacion TipoPeriodo { get; set; }
    public int NumeroPeriodos { get; set; }

    public EstadoContrato Estado { get; set; } = EstadoContrato.Activo;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Responsables por ID de usuario
    public int ResidenteUsuarioId { get; set; }
    public Usuario? ResidenteUsuario { get; set; }

    public int? SupervisorExternoUsuarioId { get; set; }
    public Usuario? SupervisorExternoUsuario { get; set; }

    public int? SuperintendenteUsuarioId { get; set; }
    public Usuario? SuperintendenteUsuario { get; set; }

    public int? FinancialUsuarioId { get; set; }
    public Usuario? FinancialUsuario { get; set; }

    // Conservados como snapshot para CaratulaBitacora; no usar en nuevos DTOs
    [Obsolete("Usar ResidenteUsuarioId + ResidenteUsuario.Nombre")]
    public string? ResidenteNombre { get; set; }
    [Obsolete("Usar SupervisorExternoUsuarioId + SupervisorExternoUsuario.Nombre")]
    public string? SupervisorExternoNombre { get; set; }
    [Obsolete("Usar SuperintendenteUsuarioId + SuperintendenteUsuario.Nombre")]
    public string? SuperintendenteNombre { get; set; }

    public ICollection<SeccionConcepto> SeccionesConcepto { get; set; } = new List<SeccionConcepto>();
    public ICollection<ConceptoContrato> ConceptoContratos { get; set; } = new List<ConceptoContrato>();
    public ICollection<PeriodoContrato> Periodos { get; set; } = new List<PeriodoContrato>();
    public ICollection<ProgramaObraItem> ProgramaObra { get; set; } = new List<ProgramaObraItem>();
    public ICollection<ProgramaObraSeccion> ProgramaObraSecciones { get; set; } = new List<ProgramaObraSeccion>();
    public ICollection<Garantia> Garantias { get; set; } = new List<Garantia>();
    public ICollection<DocumentoContrato> Documentos { get; set; } = new List<DocumentoContrato>();
    public ICollection<UsuarioContrato> Usuarios { get; set; } = new List<UsuarioContrato>();
    public ICollection<ConvenioModificatorio> Convenios { get; set; } = new List<ConvenioModificatorio>();
}
