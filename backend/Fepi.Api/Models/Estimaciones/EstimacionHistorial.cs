namespace Fepi.Api.Models;
public class EstimacionHistorial
{
    public int Id { get; set; }
    public int EstimacionId { get; set; }
    public Estimacion? Estimacion { get; set; }

    public EstadoEstimacion EstadoAnterior { get; set; }
    public EstadoEstimacion EstadoNuevo { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string? Comentario { get; set; }
}
