namespace Fepi.Api.Models;

public class RegistroDiario
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public DateOnly Fecha { get; set; }
    public int ResponsableUsuarioId { get; set; }
    public Usuario? ResponsableUsuario { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
