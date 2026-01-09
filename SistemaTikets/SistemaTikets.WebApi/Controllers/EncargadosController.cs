using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Encargados;
using SistemaTikets.Application.Services;
using System.Security.Claims;

namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EncargadosController : ControllerBase
{
    private readonly IEncargadoService _encargadoService;
    private readonly ILogger<EncargadosController> _logger;

    public EncargadosController(
        IEncargadoService encargadoService,
        ILogger<EncargadosController> logger)
    {
        _encargadoService = encargadoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los encargados (Solo Admin)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<EncargadoDto>>> GetAll()
    {
        try
        {
            var encargados = await _encargadoService.GetAllAsync();
            return Ok(encargados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encargados");
            return StatusCode(500, new { message = "Error al obtener encargados", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener encargado por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EncargadoDto>> GetById(int id)
    {
        try
        {
            var encargado = await _encargadoService.GetByIdAsync(id);
            if (encargado == null)
                return NotFound(new { message = "Encargado no encontrado" });

            return Ok(encargado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encargado {Id}", id);
            return StatusCode(500, new { message = "Error al obtener encargado", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener áreas donde un usuario es encargado
    /// </summary>
    [HttpGet("usuario/{idUsuario}")]
    public async Task<ActionResult<IEnumerable<EncargadoDto>>> GetByUsuario(int idUsuario)
    {
        try
        {
            var encargados = await _encargadoService.GetByUsuarioAsync(idUsuario);
            return Ok(encargados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encargados del usuario {IdUsuario}", idUsuario);
            return StatusCode(500, new { message = "Error al obtener encargados del usuario", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener encargados de un área específica
    /// </summary>
    [HttpGet("area/{idArea}")]
    public async Task<ActionResult<IEnumerable<EncargadoDto>>> GetByArea(int idArea)
    {
        try
        {
            var encargados = await _encargadoService.GetByAreaAsync(idArea);
            return Ok(encargados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encargados del área {IdArea}", idArea);
            return StatusCode(500, new { message = "Error al obtener encargados del área", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener solo encargados activos de un área
    /// </summary>
    [HttpGet("area/{idArea}/activos")]
    public async Task<ActionResult<IEnumerable<EncargadoDto>>> GetEncargadosActivos(int idArea)
    {
        try
        {
            var encargados = await _encargadoService.GetEncargadosActivosPorAreaAsync(idArea);
            return Ok(encargados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encargados activos del área {IdArea}", idArea);
            return StatusCode(500, new { message = "Error al obtener encargados activos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener gestores disponibles para asignar solicitudes (para encargados)
    /// </summary>
    [HttpGet("area/{idArea}/gestores-disponibles")]
    public async Task<ActionResult<IEnumerable<GestorDisponibleDto>>> GetGestoresDisponibles(int idArea)
    {
        try
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var gestores = await _encargadoService.GetGestoresDisponiblesParaAsignarAsync(idArea, idUsuario);
            return Ok(gestores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener gestores disponibles del área {IdArea}", idArea);
            return StatusCode(500, new { message = "Error al obtener gestores disponibles", error = ex.Message });
        }
    }

    /// <summary>
    /// Verificar si un usuario es encargado de un área
    /// </summary>
    [HttpGet("verificar/{idUsuario}/{idArea}")]
    public async Task<ActionResult<bool>> EsEncargado(int idUsuario, int idArea)
    {
        try
        {
            var esEncargado = await _encargadoService.EsEncargadoDeAreaAsync(idUsuario, idArea);
            return Ok(new { esEncargado });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar encargado");
            return StatusCode(500, new { message = "Error al verificar encargado", error = ex.Message });
        }
    }

    /// <summary>
    /// Crear nuevo encargado (Solo Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EncargadoDto>> Create([FromBody] CreateEncargadoRequest request)
    {
        try
        {
            var encargado = await _encargadoService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = encargado.IdEncargado }, encargado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear encargado");
            return StatusCode(500, new { message = "Error al crear encargado", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar encargado (activar/desactivar) (Solo Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EncargadoDto>> Update(int id, [FromBody] UpdateEncargadoRequest request)
    {
        try
        {
            var encargado = await _encargadoService.UpdateAsync(id, request);
            return Ok(encargado);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar encargado {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar encargado", error = ex.Message });
        }
    }

    /// <summary>
    /// Eliminar encargado (Solo Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _encargadoService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar encargado {Id}", id);
            return StatusCode(500, new { message = "Error al eliminar encargado", error = ex.Message });
        }
    }
}
