namespace Fepi.Api.Models;

public class ConvenioHistorial
{
    public int Id { get; set; }
    public int ConvenioModificatorioId { get; set; }
    public ConvenioModificatorio? ConvenioModificatorio { get; set; }

    public string Accion { get; set; } = null!;
    public EstadoConvenio? EstadoNuevo { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string? Comentario { get; set; }
}
