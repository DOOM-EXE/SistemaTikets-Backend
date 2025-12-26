namespace SistemaDeTikets.Application.DTOs.Catalogos;

public class EstadoDto
{
    public int IdEstado { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CreateEstadoRequest
{
    public string Nombre { get; set; } = string.Empty;
}
