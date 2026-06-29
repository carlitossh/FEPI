namespace Fepi.Api.Models;
public class BitacoraFirma
{
    public int Id { get; set; }
    public int BitacoraNotaId { get; set; }
    public BitacoraNota? BitacoraNota { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public RolSistema RolFirmante { get; set; }
    public bool EsEmisor { get; set; }
    public int OrdenFirma { get; set; }
    public bool Firmado { get; set; }
    public DateTime? FechaFirma { get; set; }
}
