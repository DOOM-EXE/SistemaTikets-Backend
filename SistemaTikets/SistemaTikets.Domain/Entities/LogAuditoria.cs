namespace SistemaTikets.Domain.Entities;

public class LogAuditoria
{
    public int IdLog { get; set; }
    public int? IdUsuario { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
    public string EntidadAfectada { get; set; } = string.Empty;
    public int? IdEntidad { get; set; }
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? IpOrigen { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Resultado { get; set; } = "Exitoso";
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;

    public Usuario? Usuario { get; set; }
}
