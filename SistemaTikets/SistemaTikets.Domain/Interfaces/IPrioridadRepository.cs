using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface IPrioridadRepository
{
    Task<Prioridad?> GetByIdAsync(int id);
    Task<IEnumerable<Prioridad>> GetAllAsync();
    Task<Prioridad> AddAsync(Prioridad prioridad);
    Task UpdateAsync(Prioridad prioridad);
    Task DeleteAsync(int id);
}
