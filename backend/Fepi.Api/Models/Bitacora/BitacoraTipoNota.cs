namespace Fepi.Api.Models;

public class BitacoraTipoNota
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<BitacoraNota> Notas { get; set; }
        = new List<BitacoraNota>();
}