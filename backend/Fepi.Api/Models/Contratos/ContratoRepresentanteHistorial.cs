namespace Fepi.Api.Models;

public class ContratoRepresentanteHistorial
{
    public int Id { get; set; }

    public int ContratoId { get; set; }
    public Contrato Contrato { get; set; } = null!;

    public int RepresentanteUsuarioId { get; set; }
    public Usuario RepresentanteUsuario { get; set; } = null!;

    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
}