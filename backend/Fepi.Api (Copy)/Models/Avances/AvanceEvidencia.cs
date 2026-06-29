namespace Fepi.Api.Models;

public class AvanceEvidencia
{
    public int Id { get; set; }
    public int AvanceDiarioId { get; set; }
    public AvanceDiario? AvanceDiario { get; set; }

    public string UrlFoto { get; set; } = null!;
}
