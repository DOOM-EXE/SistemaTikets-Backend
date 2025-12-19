using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface ITipoSolicitudRepository
{
    Task<TipoSolicitud?> GetByIdAsync(int id);
    Task<IEnumerable<TipoSolicitud>> GetAllAsync();
    Task<IEnumerable<TipoSolicitud>> GetByAreaAsync(int idArea);
    Task<TipoSolicitud> AddAsync(TipoSolicitud tipoSolicitud);
    Task UpdateAsync(TipoSolicitud tipoSolicitud);
    Task DeleteAsync(int id);
}
