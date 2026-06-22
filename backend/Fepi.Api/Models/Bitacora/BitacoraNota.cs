namespace Fepi.Api.Models;
public class BitacoraNota
{
    public int Id { get; set; }
    public int BitacoraId { get; set; }
    public Bitacora? Bitacora { get; set; }

    public int Folio { get; set; }
    public TipoNotaBitacora TipoRegistro { get; set; } = TipoNotaBitacora.Nota;
    public int? TipoNotaCatalogoId { get; set; }
    public BitacoraTipoNota? TipoNotaCatalogo { get; set; }

    public string Asunto { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public int? FolioVinculadoId { get; set; }
    public BitacoraNota? FolioVinculado { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public bool Cerrada => Firmas.Count(f => f.Firmado) >= 3;

    public ICollection<BitacoraFirma> Firmas { get; set; } = new List<BitacoraFirma>();
}