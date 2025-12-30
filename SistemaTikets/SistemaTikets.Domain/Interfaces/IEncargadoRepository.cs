using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface IEncargadoRepository
{
    Task<IEnumerable<Encargado>> GetAllAsync();
    Task<Encargado?> GetByIdAsync(int id);
    Task<IEnumerable<Encargado>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<Encargado>> GetByAreaAsync(int idArea);
    Task<IEnumerable<Encargado>> GetEncargadosActivosPorAreaAsync(int idArea);
    Task<bool> EsEncargadoDeAreaAsync(int idUsuario, int idArea);
    Task<Encargado> AddAsync(Encargado encargado);
    Task UpdateAsync(Encargado encargado);
    Task DeleteAsync(int id);
    Task<bool> ExisteEncargadoAsync(int idUsuario, int idArea);
}
