namespace SistemaDeTikets.Application.DTOs.Usuarios;

public class UpdateUsuarioRequest
{
    public string NombreCompleto { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public int? IdAreaAsignada { get; set; }
    public string? NewPassword { get; set; }
}
