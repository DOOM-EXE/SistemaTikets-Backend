using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int IdRol { get; set; }
    public int? IdAreaAsignada { get; set; }

    public int? IdCreadoPor { get; set; }
    public DateTime FechaCreacionUsuario { get; set; } = DateTime.UtcNow;

    public Rol Rol { get; set; } = null!;
    public Area? AreaAsignada { get; set; }
    public Usuario? CreadoPor { get; set; }

    public ICollection<Usuario> UsuariosCreados { get; set; } = new List<Usuario>();
    public ICollection<Solicitud> SolicitudesCreadas { get; set; } = new List<Solicitud>();
    public ICollection<Solicitud> SolicitudesAsignadas { get; set; } = new List<Solicitud>();
    public ICollection<Solicitud> SolicitudesAsignadasPor { get; set; } = new List<Solicitud>();
    public ICollection<TrazabilidadSolicitud> Trazabilidades { get; set; } = new List<TrazabilidadSolicitud>();
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
