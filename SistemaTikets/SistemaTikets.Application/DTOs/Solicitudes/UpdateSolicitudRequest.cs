namespace SistemaTikets.Application.DTOs.Solicitudes;

public class UpdateSolicitudRequest
{
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int IdPrioridad { get; set; }
}
