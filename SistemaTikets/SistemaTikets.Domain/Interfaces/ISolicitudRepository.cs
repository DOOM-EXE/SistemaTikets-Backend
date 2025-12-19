using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface ISolicitudRepository
{
    Task<Solicitud?> GetByIdAsync(int id);
    Task<Solicitud?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Solicitud>> GetAllAsync();
    Task<IEnumerable<Solicitud>> GetByFiltrosAsync(
        int? idEstado = null,
        int? idArea = null,
        int? idPrioridad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        string? busqueda = null
    );
    Task<IEnumerable<Solicitud>> GetBySolicitanteAsync(int idSolicitante);
    Task<IEnumerable<Solicitud>> GetByGestorAsync(int idGestor);
    Task<IEnumerable<Solicitud>> GetByAreaAsync(int idArea);
    Task<Solicitud> AddAsync(Solicitud solicitud);
    Task UpdateAsync(Solicitud solicitud);
    Task DeleteAsync(int id);
}
