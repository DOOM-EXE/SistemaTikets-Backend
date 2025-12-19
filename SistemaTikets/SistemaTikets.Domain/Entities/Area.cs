namespace SistemaTikets.Domain.Entities;

public class Area
{
    public int IdArea { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<TipoSolicitud> TiposSolicitud { get; set; } = new List<TipoSolicitud>();
    public ICollection<Usuario> UsuariosAsignados { get; set; } = new List<Usuario>();
    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
