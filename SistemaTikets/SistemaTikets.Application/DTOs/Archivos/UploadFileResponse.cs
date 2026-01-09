namespace SistemaTikets.Application.DTOs.Archivos;

public class UploadFileResponse
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
