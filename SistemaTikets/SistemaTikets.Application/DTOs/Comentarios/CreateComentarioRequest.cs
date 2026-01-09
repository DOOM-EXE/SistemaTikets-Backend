namespace SistemaTikets.Application.DTOs.Comentarios;

public class CreateComentarioRequest
{
    public int IdSolicitud { get; set; }
    public string Texto { get; set; } = string.Empty;
}
