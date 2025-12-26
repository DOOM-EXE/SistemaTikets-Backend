namespace SistemaDeTikets.Application.DTOs.Catalogos;

public class PrioridadDto
{
    public int IdPrioridad { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CreatePrioridadRequest
{
    public string Nombre { get; set; } = string.Empty;
}
