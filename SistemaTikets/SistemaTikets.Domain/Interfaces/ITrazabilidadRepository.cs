using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface ITrazabilidadRepository
{
    Task<IEnumerable<TrazabilidadSolicitud>> GetBySolicitudAsync(int idSolicitud);
    Task<TrazabilidadSolicitud> AddAsync(TrazabilidadSolicitud trazabilidad);
}
