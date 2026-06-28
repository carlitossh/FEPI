namespace Fepi.Api.Models;
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public RolUsuario Rol { get; set; }
    public bool Activo { get; set; } = true;
}
