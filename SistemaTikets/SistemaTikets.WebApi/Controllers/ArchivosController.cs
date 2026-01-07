using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Archivos;

namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArchivosController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ArchivosController> _logger;
    private const long MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB

    public ArchivosController(
        IWebHostEnvironment environment,
        ILogger<ArchivosController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No se proporcionó ningún archivo" });

            if (file.Length > MAX_FILE_SIZE)
                return BadRequest(new { message = "El archivo es demasiado grande (máximo 10MB)" });

            // Validar extensión
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xlsx", ".txt" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Tipo de archivo no permitido" });

            // Crear directorio si no existe
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "solicitudes");
            Directory.CreateDirectory(uploadsFolder);

            // Generar nombre con timestamp corto + nombre original
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            // Limpiar caracteres no válidos del nombre original
            var cleanFileName = string.Join("_", originalFileName.Split(Path.GetInvalidFileNameChars()));
            var uniqueFileName = $"{timestamp}_{cleanFileName}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retornar URL
            var fileUrl = $"/uploads/solicitudes/{uniqueFileName}";

            _logger.LogInformation("Archivo subido exitosamente: {FileName}", uniqueFileName);

            return Ok(new UploadFileResponse
            {
                FileName = file.FileName,
                FileUrl = fileUrl,
                FileSize = file.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir archivo");
            return StatusCode(500, new { message = "Error al subir el archivo" });
        }
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "solicitudes", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Archivo no encontrado" });

            // Leer el archivo
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Determinar el Content-Type según la extensión
            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };

            _logger.LogInformation("Archivo descargado: {FileName} por usuario {UserId}", 
                fileName, 
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar archivo");
            return StatusCode(500, new { message = "Error al descargar el archivo" });
        }
    }

    [HttpGet("view/{fileName}")]
    public async Task<IActionResult> ViewFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "solicitudes", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Archivo no encontrado" });

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };

            // Retornar inline (para ver en el navegador) en lugar de forzar descarga
            return File(fileBytes, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al visualizar archivo");
            return StatusCode(500, new { message = "Error al visualizar el archivo" });
        }
    }

    [HttpDelete("{fileName}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "solicitudes", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Archivo no encontrado" });

            System.IO.File.Delete(filePath);
            
            _logger.LogInformation("Archivo eliminado: {FileName}", fileName);
            
            return Ok(new { message = "Archivo eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar archivo");
            return StatusCode(500, new { message = "Error al eliminar el archivo" });
        }
    }
}
