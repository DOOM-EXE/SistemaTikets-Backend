namespace SistemaTikets.Domain.Entities;

public class Encargado
{
    public int IdEncargado { get; set; }
    public int IdUsuario { get; set; }
    public int IdArea { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    public Usuario Usuario { get; set; } = null!;
    public Area Area { get; set; } = null!;
}
