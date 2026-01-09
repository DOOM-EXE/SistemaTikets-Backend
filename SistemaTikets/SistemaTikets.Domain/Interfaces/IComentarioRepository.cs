using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface IComentarioRepository
{
    Task<IEnumerable<Comentario>> GetBySolicitudAsync(int idSolicitud);
    Task<Comentario> AddAsync(Comentario comentario);
    Task DeleteAsync(int id);
}
