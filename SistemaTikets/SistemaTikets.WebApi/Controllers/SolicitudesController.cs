using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Solicitudes;
using SistemaTikets.Application.Services;

namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudService _solicitudService;

    public SolicitudesController(ISolicitudService solicitudService)
    {
        _solicitudService = solicitudService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? idEstado = null,
        [FromQuery] int? idArea = null,
        [FromQuery] int? idPrioridad = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] string? busqueda = null)
    {
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        IEnumerable<SolicitudDto> solicitudes;

        if (rol == "Admin")
        {
            // Admin ve todas con filtros
            solicitudes = await _solicitudService.GetByFiltrosAsync(
                idEstado, idArea, idPrioridad, fechaDesde, fechaHasta, busqueda);
        }
        else if (rol == "Gestor")
        {
            // Gestor ve las de su área
            var idAreaGestor = int.Parse(User.FindFirst("IdArea")?.Value ?? "0");
            solicitudes = await _solicitudService.GetByAreaAsync(idAreaGestor);
        }
        else // Solicitante
        {
            // Solicitante solo ve las suyas
            solicitudes = await _solicitudService.GetBySolicitanteAsync(idUsuario);
        }

        return Ok(solicitudes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var solicitud = await _solicitudService.GetDetalleAsync(id);

        if (solicitud == null)
            return NotFound(new { message = "Solicitud no encontrada" });

        // Validar que el usuario tenga permiso para ver la solicitud
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        if (rol == "Solicitante" && solicitud.Solicitud.IdSolicitante != idUsuario)
            return Forbid();

        if (rol == "Gestor")
        {
            var idAreaGestor = int.Parse(User.FindFirst("IdArea")?.Value ?? "0");
            if (solicitud.Solicitud.IdArea != idAreaGestor)
                return Forbid();
        }

        return Ok(solicitud);
    }

    [HttpGet("mis-solicitudes")]
    [Authorize(Roles = "Solicitante")]
    public async Task<IActionResult> GetMisSolicitudes()
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var solicitudes = await _solicitudService.GetBySolicitanteAsync(idUsuario);
        return Ok(solicitudes);
    }

    [HttpGet("asignadas-a-mi")]
    [Authorize(Roles = "Gestor")]
    public async Task<IActionResult> GetAsignadasAMi()
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var solicitudes = await _solicitudService.GetByGestorAsync(idUsuario);
        return Ok(solicitudes);
    }

    [HttpPost]
    [Authorize(Roles = "Solicitante")]
    public async Task<IActionResult> Create([FromBody] CreateSolicitudRequest request)
    {
        try
        {
            var idSolicitante = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var solicitud = await _solicitudService.CreateAsync(request, idSolicitante);
            return CreatedAtAction(nameof(GetDetalle), new { id = solicitud.IdSolicitud }, solicitud);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Solicitante")] // Solo el Solicitante puede editar
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSolicitudRequest request)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Obtener la solicitud para validar permisos
            var solicitudDetalle = await _solicitudService.GetDetalleAsync(id);
            
            if (solicitudDetalle == null)
                return NotFound(new { message = "Solicitud no encontrada" });
            
            // Validar que el usuario sea el solicitante
            if (solicitudDetalle.Solicitud.IdSolicitante != idUsuario)
                return Forbid();
            
            // Validar que la solicitud esté en estado "Nueva" (ID = 1)
            if (solicitudDetalle.Solicitud.IdEstado != 1)
                return BadRequest(new { message = "Solo se puede editar una solicitud en estado 'Nueva'" });
            
            var solicitud = await _solicitudService.UpdateAsync(id, request, idUsuario);
            return Ok(solicitud);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cambiar-estado")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoRequest request)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _solicitudService.CambiarEstadoAsync(id, request, idUsuario);
            return Ok(new { message = "Estado cambiado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/asignar-gestor")]
    [Authorize] // Permitir a usuarios autenticados, validación de permisos en service
    public async Task<IActionResult> AsignarGestor(int id, [FromBody] AsignarGestorRequest request)
    {
        try
        {
            var idAsignadoPor = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _solicitudService.AsignarGestorAsync(id, request, idAsignadoPor);
            return Ok(new { message = "Gestor asignado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/tomar")]
    [Authorize(Roles = "Gestor")]
    public async Task<IActionResult> TomarSolicitud(int id)
    {
        try
        {
            var idGestor = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _solicitudService.TomarSolicitudAsync(id, idGestor);
            return Ok(new { message = "Solicitud tomada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _solicitudService.DeleteAsync(id);
        return NoContent();
    }
}
