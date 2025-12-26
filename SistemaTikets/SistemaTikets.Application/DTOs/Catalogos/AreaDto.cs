namespace SistemaTikets.Application.DTOs.Catalogos;

public class AreaDto
{
    public int IdArea { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CreateAreaRequest
{
    public string Nombre { get; set; } = string.Empty;
}
