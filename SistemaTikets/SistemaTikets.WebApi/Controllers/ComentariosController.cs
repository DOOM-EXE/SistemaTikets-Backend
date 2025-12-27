using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Comentarios;
using SistemaTikets.Application.Services;

namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _comentarioService;

    public ComentariosController(IComentarioService comentarioService)
    {
        _comentarioService = comentarioService;
    }

    [HttpGet("solicitud/{idSolicitud}")]
    public async Task<IActionResult> GetBySolicitud(int idSolicitud)
    {
        var comentarios = await _comentarioService.GetBySolicitudAsync(idSolicitud);
        return Ok(comentarios);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComentarioRequest request)
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var comentario = await _comentarioService.AddAsync(request, idUsuario);
        return CreatedAtAction(nameof(GetBySolicitud), new { idSolicitud = comentario.IdComentario }, comentario);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _comentarioService.DeleteAsync(id);
        return NoContent();
    }
}
