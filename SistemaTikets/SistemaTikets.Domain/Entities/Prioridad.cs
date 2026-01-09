namespace SistemaTikets.Domain.Entities;

public class Prioridad
{
    public int IdPrioridad { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
