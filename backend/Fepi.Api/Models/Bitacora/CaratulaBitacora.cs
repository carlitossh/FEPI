namespace Fepi.Api.Models;
public class CaratulaBitacora
{
    public int Id { get; set; }

    public int BitacoraId { get; set; }
    public Bitacora? Bitacora { get; set; }

    public string FolioBitacora { get; set; } = string.Empty;

    public string NombreContrato { get; set; } = string.Empty;

    public string NumeroContrato { get; set; } = string.Empty;

    public TipoContrato TipoContrato { get; set; }

    public string Dependencia { get; set; } = string.Empty;

    public string Contratista { get; set; } = string.Empty;

    public string Residente { get; set; } = string.Empty;

    public string Supervisor { get; set; } = string.Empty;

    public string Superintendente { get; set; } = string.Empty;

    public DateTime FechaApertura { get; set; }
}