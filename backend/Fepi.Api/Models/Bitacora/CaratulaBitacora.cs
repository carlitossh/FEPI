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

    // Snapshot de importes al momento de apertura
    public decimal MontoContratadoConIVA { get; set; }
    public decimal MontoContratadoSinIVA { get; set; }

    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaTerminoProgramada { get; set; }

    // Referencias a usuarios por nombre (snapshot de apertura)
    public string Residente { get; set; } = string.Empty;
    public string Supervisor { get; set; } = string.Empty;
    public string Superintendente { get; set; } = string.Empty;

    // Referencias opcionales a usuarios por Id
    public int? ResidenteObraUsuarioId { get; set; }
    public Usuario? ResidenteObraUsuario { get; set; }

    public int? SuperintendenteUsuarioId { get; set; }
    public Usuario? SuperintendenteUsuario { get; set; }

    public int? SupervisorObraUsuarioId { get; set; }
    public Usuario? SupervisorObraUsuario { get; set; }

    public DateTime FechaApertura { get; set; }
    public bool Abierta { get; set; } = true;
}
