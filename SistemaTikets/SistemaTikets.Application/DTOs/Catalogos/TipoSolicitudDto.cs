namespace SistemaDeTikets.Application.DTOs.Catalogos;

public class TipoSolicitudDto
{
    public int IdTipoSolicitud { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdArea { get; set; }
    public string NombreArea { get; set; } = string.Empty;
}

public class CreateTipoSolicitudRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int IdArea { get; set; }
}
