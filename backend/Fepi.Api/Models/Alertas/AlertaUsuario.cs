namespace Fepi.Api.Models;

public class AlertaUsuario
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public int? ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public string EntidadRelacionada { get; set; } = null!;
    public int? EntidadId { get; set; }

    public string Titulo { get; set; } = null!;
    public string Mensaje { get; set; } = null!;

    public TipoAlerta Tipo { get; set; }
    public PrioridadAlerta Prioridad { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool Leida { get; set; }
    public DateTime? FechaLectura { get; set; }
}
