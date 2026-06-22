namespace Fepi.Api.Models;
public class BitacoraMinuta
{
    public int Id { get; set; }
    public int BitacoraId { get; set; }
    public Bitacora? Bitacora { get; set; }

    public DateOnly Fecha { get; set; }
    public string Lugar { get; set; } = null!;
    public string ContenidoAcuerdos { get; set; } = null!;

    public ICollection<BitacoraMinutaParticipante> Participantes { get; set; } = new List<BitacoraMinutaParticipante>();
}
