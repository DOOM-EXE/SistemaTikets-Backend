using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.Services;

namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditoriaController : ControllerBase
{
    private readonly ILogAuditoriaService _logService;

    public AuditoriaController(ILogAuditoriaService logService)
    {
        _logService = logService;
    }

    // Obtener todos los registros de auditoría
    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await _logService.GetAllLogsAsync(1, int.MaxValue);
        return Ok(logs);
    }

    // Obtener registros filtrados por idUsuario
    [HttpGet("usuario/{idUsuario}")]
    public async Task<IActionResult> GetLogsByUsuario(int idUsuario)
    {
        var logs = await _logService.GetLogsByUsuarioAsync(idUsuario);
        return Ok(logs);
    }
}
