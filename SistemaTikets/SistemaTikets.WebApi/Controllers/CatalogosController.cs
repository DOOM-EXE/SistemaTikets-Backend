using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Catalogos;
using SistemaTikets.Application.Services;
using SistemaTikets.WebApi.Helpers;

namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogoService _catalogoService;

    public CatalogosController(ICatalogoService catalogoService)
    {
        _catalogoService = catalogoService;
    }

    // ============ AREAS ============
    [HttpGet("areas")]
    public async Task<IActionResult> GetAllAreas()
    {
        var areas = await _catalogoService.GetAllAreasAsync();
        return Ok(areas);
    }

    [HttpGet("areas/{id}")]
    public async Task<IActionResult> GetAreaById(int id)
    {
        var area = await _catalogoService.GetAreaByIdAsync(id);
        if (area == null)
            return NotFound();
        return Ok(area);
    }

    [HttpPost("areas")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateArea([FromBody] CreateAreaRequest request)
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
        var area = await _catalogoService.CreateAreaAsync(request, idUsuario, ipAddress);
        return CreatedAtAction(nameof(GetAreaById), new { id = area.IdArea }, area);
    }

    [HttpPut("areas/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateArea(int id, [FromBody] CreateAreaRequest request)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            var area = await _catalogoService.UpdateAreaAsync(id, request, idUsuario, ipAddress);
            return Ok(area);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("areas/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteArea(int id)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            await _catalogoService.DeleteAreaAsync(id, idUsuario, ipAddress);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ============ TIPOS DE SOLICITUD ============
    [HttpGet("tipos-solicitud")]
    public async Task<IActionResult> GetAllTiposSolicitud()
    {
        var tipos = await _catalogoService.GetAllTiposSolicitudAsync();
        return Ok(tipos);
    }

    [HttpGet("tipos-solicitud/area/{idArea}")]
    public async Task<IActionResult> GetTiposSolicitudByArea(int idArea)
    {
        var tipos = await _catalogoService.GetTiposSolicitudByAreaAsync(idArea);
        return Ok(tipos);
    }

    [HttpGet("tipos-solicitud/{id}")]
    public async Task<IActionResult> GetTipoSolicitudById(int id)
    {
        var tipo = await _catalogoService.GetTipoSolicitudByIdAsync(id);
        if (tipo == null)
            return NotFound();
        return Ok(tipo);
    }

    [HttpPost("tipos-solicitud")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTipoSolicitud([FromBody] CreateTipoSolicitudRequest request)
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
        var tipo = await _catalogoService.CreateTipoSolicitudAsync(request, idUsuario, ipAddress);
        return CreatedAtAction(nameof(GetTipoSolicitudById), new { id = tipo.IdTipoSolicitud }, tipo);
    }

    [HttpPut("tipos-solicitud/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTipoSolicitud(int id, [FromBody] CreateTipoSolicitudRequest request)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            var tipo = await _catalogoService.UpdateTipoSolicitudAsync(id, request, idUsuario, ipAddress);
            return Ok(tipo);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("tipos-solicitud/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTipoSolicitud(int id)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            await _catalogoService.DeleteTipoSolicitudAsync(id, idUsuario, ipAddress);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ============ PRIORIDADES ============
    [HttpGet("prioridades")]
    public async Task<IActionResult> GetAllPrioridades()
    {
        var prioridades = await _catalogoService.GetAllPrioridadesAsync();
        return Ok(prioridades);
    }

    [HttpGet("prioridades/{id}")]
    public async Task<IActionResult> GetPrioridadById(int id)
    {
        var prioridad = await _catalogoService.GetPrioridadByIdAsync(id);
        if (prioridad == null)
            return NotFound();
        return Ok(prioridad);
    }

    [HttpPost("prioridades")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePrioridad([FromBody] CreatePrioridadRequest request)
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
        var prioridad = await _catalogoService.CreatePrioridadAsync(request, idUsuario, ipAddress);
        return CreatedAtAction(nameof(GetPrioridadById), new { id = prioridad.IdPrioridad }, prioridad);
    }

    [HttpPut("prioridades/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePrioridad(int id, [FromBody] CreatePrioridadRequest request)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            var prioridad = await _catalogoService.UpdatePrioridadAsync(id, request, idUsuario, ipAddress);
            return Ok(prioridad);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("prioridades/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePrioridad(int id)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            await _catalogoService.DeletePrioridadAsync(id, idUsuario, ipAddress);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ============ ESTADOS ============
    [HttpGet("estados")]
    public async Task<IActionResult> GetAllEstados()
    {
        var estados = await _catalogoService.GetAllEstadosAsync();
        return Ok(estados);
    }

    [HttpGet("estados/{id}")]
    public async Task<IActionResult> GetEstadoById(int id)
    {
        var estado = await _catalogoService.GetEstadoByIdAsync(id);
        if (estado == null)
            return NotFound();
        return Ok(estado);
    }

    [HttpPost("estados")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateEstado([FromBody] CreateEstadoRequest request)
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
        var estado = await _catalogoService.CreateEstadoAsync(request, idUsuario, ipAddress);
        return CreatedAtAction(nameof(GetEstadoById), new { id = estado.IdEstado }, estado);
    }

    [HttpPut("estados/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] CreateEstadoRequest request)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            var estado = await _catalogoService.UpdateEstadoAsync(id, request, idUsuario, ipAddress);
            return Ok(estado);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("estados/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEstado(int id)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            await _catalogoService.DeleteEstadoAsync(id, idUsuario, ipAddress);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ============ ROLES ============
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _catalogoService.GetAllRolesAsync();
        return Ok(roles);
    }
}
