using SistemaTikets.Application.DTOs.Comentarios;
using SistemaTikets.Application.DTOs.Solicitudes;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;


namespace SistemaTikets.Application.Services;

public interface IComentarioService
{
    Task<IEnumerable<ComentarioDto>> GetBySolicitudAsync(int idSolicitud);
    Task<ComentarioDto> AddAsync(CreateComentarioRequest request, int idUsuario, string? ipAddress = null);
    Task DeleteAsync(int id);
}

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService? _auditoriaService;

    public ComentarioService(
        IComentarioRepository comentarioRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService? auditoriaService = null)
    {
        _comentarioRepository = comentarioRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
    }

    public async Task<IEnumerable<ComentarioDto>> GetBySolicitudAsync(int idSolicitud)
    {
        var comentarios = await _comentarioRepository.GetBySolicitudAsync(idSolicitud);
        return comentarios.Select(c => new ComentarioDto
        {
            IdComentario = c.IdComentario,
            Texto = c.Texto,
            FechaComentario = c.FechaComentario,
            IdUsuario = c.IdUsuario ?? 0,
            NombreUsuario = c.Usuario?.NombreCompleto ?? "(Usuario eliminado)",
            RolUsuario = c.Usuario?.Rol?.Nombre,
            EsEncargado = c.Usuario?.EncargadosDeAreas?.Any(e => e.Activo) ?? false
        });
    }

    public async Task<ComentarioDto> AddAsync(CreateComentarioRequest request, int idUsuario, string? ipAddress = null)
    {
        var comentario = new Comentario
        {
            IdSolicitud = request.IdSolicitud,
            IdUsuario = idUsuario,
            Texto = request.Texto,
            FechaComentario = DateTime.UtcNow
        };

        var created = await _comentarioRepository.AddAsync(comentario);
        
        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarCreacionComentario(idUsuario, request.IdSolicitud, request.Texto, ipAddress);
        }

        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);

        return new ComentarioDto
        {
            IdComentario = created.IdComentario,
            Texto = created.Texto,
            FechaComentario = created.FechaComentario,
            IdUsuario = created.IdUsuario ?? 0,
            NombreUsuario = usuario?.NombreCompleto ?? "(Usuario eliminado)",
            RolUsuario = usuario?.Rol?.Nombre,
            EsEncargado = usuario?.EncargadosDeAreas?.Any(e => e.Activo) ?? false
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _comentarioRepository.DeleteAsync(id);
    }
}
