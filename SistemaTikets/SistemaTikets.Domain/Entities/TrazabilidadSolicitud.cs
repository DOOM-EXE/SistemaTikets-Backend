using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Entities;

public class TrazabilidadSolicitud
{
    public int IdTrazabilidad { get; set; }
    public int IdSolicitud { get; set; }
    public int? IdUsuarioActor { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;

    public Solicitud Solicitud { get; set; } = null!;
    public Usuario? UsuarioActor { get; set; }
}
