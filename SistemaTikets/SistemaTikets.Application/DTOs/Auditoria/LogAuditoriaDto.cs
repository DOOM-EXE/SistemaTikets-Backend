namespace SistemaTikets.Application.DTOs.Auditoria;

public class LogAuditoriaDto
{
    public int IdLog { get; set; }
    public int? IdUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
    public string EntidadAfectada { get; set; } = string.Empty;
    public int? IdEntidad { get; set; }
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? IpOrigen { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
}

public class FiltroLogsRequest
{
    public int? IdUsuario { get; set; }
    public string? TipoAccion { get; set; }
    public string? EntidadAfectada { get; set; }
    public int? IdEntidad { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Resultado { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
