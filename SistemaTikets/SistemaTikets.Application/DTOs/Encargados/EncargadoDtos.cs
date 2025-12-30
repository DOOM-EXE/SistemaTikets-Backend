namespace SistemaTikets.Application.DTOs.Encargados;

public class EncargadoDto
{
    public int IdEncargado { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public int IdArea { get; set; }
    public string NombreArea { get; set; } = string.Empty;
    public DateTime FechaAsignacion { get; set; }
    public bool Activo { get; set; }
}

public class CreateEncargadoRequest
{
    public int IdUsuario { get; set; }
    public int IdArea { get; set; }
}

public class UpdateEncargadoRequest
{
    public bool Activo { get; set; }
}

public class GestorDisponibleDto
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool EsEncargado { get; set; }
}
