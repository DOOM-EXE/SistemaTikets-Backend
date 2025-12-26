using SistemaTikets.Application.DTOs.Comentarios;
using SistemaTikets.Application.DTOs.Solicitudes;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;


namespace SistemaTikets.Application.Services;

public interface IComentarioService
{
    Task<IEnumerable<ComentarioDto>> GetBySolicitudAsync(int idSolicitud);
    Task<ComentarioDto> AddAsync(CreateComentarioRequest request, int idUsuario);
    Task DeleteAsync(int id);
}

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ComentarioService(
        IComentarioRepository comentarioRepository,
        IUsuarioRepository usuarioRepository)
    {
        _comentarioRepository = comentarioRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<ComentarioDto>> GetBySolicitudAsync(int idSolicitud)
    {
        var comentarios = await _comentarioRepository.GetBySolicitudAsync(idSolicitud);
        return comentarios.Select(c => new ComentarioDto
        {
            IdComentario = c.IdComentario,
            Texto = c.Texto,
            FechaComentario = c.FechaComentario,
            IdUsuario = c.IdUsuario,
            NombreUsuario = c.Usuario.NombreCompleto
        });
    }

    public async Task<ComentarioDto> AddAsync(CreateComentarioRequest request, int idUsuario)
    {
        var comentario = new Comentario
        {
            IdSolicitud = request.IdSolicitud,
            IdUsuario = idUsuario,
            Texto = request.Texto,
            FechaComentario = DateTime.UtcNow
        };

        var created = await _comentarioRepository.AddAsync(comentario);
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);

        return new ComentarioDto
        {
            IdComentario = created.IdComentario,
            Texto = created.Texto,
            FechaComentario = created.FechaComentario,
            IdUsuario = created.IdUsuario,
            NombreUsuario = usuario?.NombreCompleto ?? string.Empty
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _comentarioRepository.DeleteAsync(id);
    }
}
