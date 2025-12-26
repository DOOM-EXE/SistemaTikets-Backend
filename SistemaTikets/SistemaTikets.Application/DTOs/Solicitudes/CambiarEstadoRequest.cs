namespace SistemaDeTikets.Application.DTOs.Solicitudes;

public class CambiarEstadoRequest
{
    public int IdEstado { get; set; }
    public string? Comentario { get; set; }
}
