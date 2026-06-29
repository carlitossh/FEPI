namespace Fepi.Api.Models;

public class ConvenioPromocionResidencia
{
    public int Id { get; set; }
    public int ConvenioModificatorioId { get; set; }
    public ConvenioModificatorio? ConvenioModificatorio { get; set; }

    public int ResidenteId { get; set; }
    public Usuario? Residente { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
