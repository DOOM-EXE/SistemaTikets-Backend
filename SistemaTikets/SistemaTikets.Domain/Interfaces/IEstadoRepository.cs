using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface IEstadoRepository
{
    Task<Estado?> GetByIdAsync(int id);
    Task<IEnumerable<Estado>> GetAllAsync();
    Task<Estado> AddAsync(Estado estado);
    Task UpdateAsync(Estado estado);
    Task DeleteAsync(int id);
}
