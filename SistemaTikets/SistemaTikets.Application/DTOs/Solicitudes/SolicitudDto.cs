namespace SistemaTikets.Application.DTOs.Solicitudes;

public class SolicitudDto
{
    public int IdSolicitud { get; set; }
    public string? Codigo { get; set; }
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ArchivoUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
    
    public int IdSolicitante { get; set; }
    public string NombreSolicitante { get; set; } = string.Empty;
    
    public int IdArea { get; set; }
    public string NombreArea { get; set; } = string.Empty;
    
    public int IdTipoSolicitud { get; set; }
    public string NombreTipoSolicitud { get; set; } = string.Empty;
    
    public int IdPrioridad { get; set; }
    public string NombrePrioridad { get; set; } = string.Empty;
    
    public int IdEstado { get; set; }
    public string NombreEstado { get; set; } = string.Empty;
    
    public int? IdGestorAsignado { get; set; }
    public string? NombreGestorAsignado { get; set; }
    public DateTime? FechaAsignacion { get; set; }
}
