using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface IAreaRepository
{
    Task<Area?> GetByIdAsync(int id);
    Task<IEnumerable<Area>> GetAllAsync();
    Task<Area> AddAsync(Area area);
    Task UpdateAsync(Area area);
    Task DeleteAsync(int id);
}
