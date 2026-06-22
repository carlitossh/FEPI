namespace Fepi.Api.Models;
public class UsuarioContrato
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }
    public RolSistema Rol { get; set; }
}
