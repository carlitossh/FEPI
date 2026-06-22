namespace Fepi.Api.Models;

/// <summary>SV-06 — Panel persistente de alertas, según catálogo.</summary>
public class Alerta
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public TipoAlerta Tipo { get; set; }
    public int EntidadReferenciaId { get; set; }
    public string EntidadReferenciaTipo { get; set; } = null!;

    public RolSistema RolDestino { get; set; }
    public string Mensaje { get; set; } = null!;
    public EstadoAlerta Estado { get; set; } = EstadoAlerta.Activa;

    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
}
