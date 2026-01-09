namespace SistemaTikets.Application.DTOs.Solicitudes;

public class CreateSolicitudRequest
{
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ArchivoUrl { get; set; }
    public int IdArea { get; set; }
    public int IdTipoSolicitud { get; set; }
    public int IdPrioridad { get; set; }
}
