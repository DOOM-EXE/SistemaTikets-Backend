namespace SistemaTikets.Application.DTOs.Usuarios;

public class CreateUsuarioRequest
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public int? IdAreaAsignada { get; set; }
}
