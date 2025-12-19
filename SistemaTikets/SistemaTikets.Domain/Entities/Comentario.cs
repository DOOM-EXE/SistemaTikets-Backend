namespace SistemaTikets.Domain.Entities;

public class Comentario
{
    public int IdComentario { get; set; }
    public int IdSolicitud { get; set; }
    public int IdUsuario { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime FechaComentario { get; set; } = DateTime.UtcNow;

    public Solicitud Solicitud { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
