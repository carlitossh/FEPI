namespace Fepi.Api.Models;
public class EstimacionNotaBitacora
{
    public int Id { get; set; }
    public int EstimacionId { get; set; }
    public Estimacion? Estimacion { get; set; }
    public int NotaBitacoraId { get; set; }
    public BitacoraNota? NotaBitacora { get; set; }
}
