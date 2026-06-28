namespace Fepi.Api.Models;

public class ConvenioCambio
{
    public int Id { get; set; }
    public int ConvenioModificatorioId { get; set; }
    public ConvenioModificatorio? ConvenioModificatorio { get; set; }

    public string EntidadAfectada { get; set; } = null!;
    public string CampoAfectado { get; set; } = null!;
    public string ValorAnterior { get; set; } = null!;
    public string ValorNuevo { get; set; } = null!;
    public string? DescripcionCambio { get; set; }
}
