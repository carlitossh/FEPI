namespace Fepi.Api.Models;
public class BitacoraTipoNota
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public CatalogoTipoNota Codigo { get; set; }
}