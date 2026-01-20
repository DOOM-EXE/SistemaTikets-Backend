using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Usuarios;
using SistemaTikets.Application.Services;
using SistemaTikets.WebApi.Helpers;
using SistemaTikets.WebApi.Filters;
using SistemaTikets.Domain.Interfaces;


namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuariosController(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository)
    {
        _usuarioService = usuarioService;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado" });

        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioRequest request)
    {
        try
        {
            var idCreador = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            var usuario = await _usuarioService.CreateAsync(request, idCreador, ipAddress);
            return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [ServiceFilter(typeof(CambiarPasswordAccessFilter))]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioRequest request)
    {
        try
        {
            var idEditor = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            var usuario = await _usuarioService.UpdateAsync(id, request, idEditor, ipAddress);
            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoUsuarioRequest request)
    {
        try
        {
            var idEditor = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
            var usuario = await _usuarioService.CambiarEstadoAsync(id, request.Activo, idEditor, ipAddress);
            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
