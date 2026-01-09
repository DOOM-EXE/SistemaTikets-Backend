using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Entities;

public class TipoSolicitud
{
    public int IdTipoSolicitud { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdArea { get; set; }

    public Area Area { get; set; } = null!;
    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
