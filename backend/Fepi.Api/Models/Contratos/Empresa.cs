namespace Fepi.Api.Models;

public class Empresa
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public int RepresentanteUsuarioId { get; set; }

    public Usuario RepresentanteUsuario { get; set; } = null!;

    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
}