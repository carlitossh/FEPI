namespace Fepi.Api.Models;

public class Bitacora
{
    public int Id { get; set; }

    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relación 1 a 1
    public CaratulaBitacora? Caratula { get; set; }

    public ICollection<BitacoraNota> Notas { get; set; } = new List<BitacoraNota>();

    public ICollection<BitacoraMinuta> Minutas { get; set; } = new List<BitacoraMinuta>();

    public ICollection<BitacoraIncidencia> Incidencias { get; set; } = new List<BitacoraIncidencia>();
    public EstadoBitacora Estado { get; set; } = EstadoBitacora.Borrador;
}