namespace SistemaTikets.Application.DTOs.Usuarios;

public class UsuarioDto
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public int? IdAreaAsignada { get; set; }
    public string? NombreArea { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime FechaCreacionUsuario { get; set; }
    public bool Activo { get; set; }
}
