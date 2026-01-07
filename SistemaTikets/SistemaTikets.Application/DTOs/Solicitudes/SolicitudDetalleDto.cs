namespace SistemaTikets.Application.DTOs.Solicitudes;

public class SolicitudDetalleDto
{
    public SolicitudDto Solicitud { get; set; } = null!;
    public List<TrazabilidadDto> Trazabilidad { get; set; } = new();
    public List<ComentarioDto> Comentarios { get; set; } = new();
}

public class TrazabilidadDto
{
    public int IdTrazabilidad { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public string? NombreUsuario { get; set; }
    public string? RolUsuario { get; set; }
    public bool? EsEncargado { get; set; }
}

public class ComentarioDto
{
    public int IdComentario { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime FechaComentario { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string? RolUsuario { get; set; }
    public bool? EsEncargado { get; set; }
}
