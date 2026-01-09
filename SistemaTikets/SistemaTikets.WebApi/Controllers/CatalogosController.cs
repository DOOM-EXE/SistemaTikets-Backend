using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Catalogos;
using SistemaTikets.Application.Services;

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
        var area = await _catalogoService.CreateAreaAsync(request);
        return CreatedAtAction(nameof(GetAreaById), new { id = area.IdArea }, area);
    }

    [HttpPut("areas/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateArea(int id, [FromBody] CreateAreaRequest request)
    {
        try
        {
            var area = await _catalogoService.UpdateAreaAsync(id, request);
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
        await _catalogoService.DeleteAreaAsync(id);
        return NoContent();
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
        var tipo = await _catalogoService.CreateTipoSolicitudAsync(request);
        return CreatedAtAction(nameof(GetTipoSolicitudById), new { id = tipo.IdTipoSolicitud }, tipo);
    }

    [HttpPut("tipos-solicitud/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTipoSolicitud(int id, [FromBody] CreateTipoSolicitudRequest request)
    {
        try
        {
            var tipo = await _catalogoService.UpdateTipoSolicitudAsync(id, request);
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
        await _catalogoService.DeleteTipoSolicitudAsync(id);
        return NoContent();
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
        var prioridad = await _catalogoService.CreatePrioridadAsync(request);
        return CreatedAtAction(nameof(GetPrioridadById), new { id = prioridad.IdPrioridad }, prioridad);
    }

    [HttpPut("prioridades/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePrioridad(int id, [FromBody] CreatePrioridadRequest request)
    {
        try
        {
            var prioridad = await _catalogoService.UpdatePrioridadAsync(id, request);
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
        await _catalogoService.DeletePrioridadAsync(id);
        return NoContent();
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
        var estado = await _catalogoService.CreateEstadoAsync(request);
        return CreatedAtAction(nameof(GetEstadoById), new { id = estado.IdEstado }, estado);
    }

    [HttpPut("estados/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] CreateEstadoRequest request)
    {
        try
        {
            var estado = await _catalogoService.UpdateEstadoAsync(id, request);
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
        await _catalogoService.DeleteEstadoAsync(id);
        return NoContent();
    }

    // ============ ROLES ============
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _catalogoService.GetAllRolesAsync();
        return Ok(roles);
    }
}
