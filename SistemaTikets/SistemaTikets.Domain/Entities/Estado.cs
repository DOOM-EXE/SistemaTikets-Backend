namespace SistemaTikets.Domain.Entities;

public class Estado
{
    public int IdEstado { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
