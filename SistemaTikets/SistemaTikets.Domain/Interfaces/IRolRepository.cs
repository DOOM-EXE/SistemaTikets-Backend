using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface IRolRepository
{
    Task<Rol?> GetByIdAsync(int id);
    Task<IEnumerable<Rol>> GetAllAsync();
    Task<Rol> AddAsync(Rol rol);
    Task UpdateAsync(Rol rol);
    Task DeleteAsync(int id);
}
