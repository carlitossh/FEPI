namespace Fepi.Api.Models;
public class BitacoraMinutaParticipante
{
    public int Id { get; set; }
    public int BitacoraMinutaId { get; set; }
    public BitacoraMinuta? BitacoraMinuta { get; set; }

    public string NombreParticipante { get; set; } = null!;
}
