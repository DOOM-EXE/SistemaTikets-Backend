namespace SistemaTikets.Domain.Entities;

public class Solicitud
{
    public int IdSolicitud { get; set; }
    public string? Codigo { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ArchivoUrl { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public int? IdSolicitante { get; set; }
    public int IdArea { get; set; }
    public int IdTipoSolicitud { get; set; }
    public int IdPrioridad { get; set; }
    public int IdEstado { get; set; }

    public int? IdGestorAsignado { get; set; }
    public int? IdAsignadoPor { get; set; }
    public DateTime? FechaAsignacion { get; set; }

    public Usuario? Solicitante { get; set; }
    public Area Area { get; set; } = null!;
    public TipoSolicitud TipoSolicitud { get; set; } = null!;
    public Prioridad Prioridad { get; set; } = null!;
    public Estado Estado { get; set; } = null!;
    public Usuario? GestorAsignado { get; set; }
    public Usuario? AsignadoPor { get; set; }

    public ICollection<TrazabilidadSolicitud> Trazabilidades { get; set; } = new List<TrazabilidadSolicitud>();
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
