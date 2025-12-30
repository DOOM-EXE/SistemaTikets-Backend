using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class EncargadoRepository : IEncargadoRepository
{
    private readonly ApplicationDbContext _context;

    public EncargadoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Encargado>> GetAllAsync()
    {
        return await _context.Encargados
            .Include(e => e.Usuario)
            .Include(e => e.Area)
            .ToListAsync();
    }

    public async Task<Encargado?> GetByIdAsync(int id)
    {
        return await _context.Encargados
            .Include(e => e.Usuario)
            .Include(e => e.Area)
            .FirstOrDefaultAsync(e => e.IdEncargado == id);
    }

    public async Task<IEnumerable<Encargado>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.Encargados
            .Include(e => e.Usuario)
            .Include(e => e.Area)
            .Where(e => e.IdUsuario == idUsuario && e.Activo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Encargado>> GetByAreaAsync(int idArea)
    {
        return await _context.Encargados
            .Include(e => e.Usuario)
            .Include(e => e.Area)
            .Where(e => e.IdArea == idArea)
            .ToListAsync();
    }

    public async Task<IEnumerable<Encargado>> GetEncargadosActivosPorAreaAsync(int idArea)
    {
        return await _context.Encargados
            .Include(e => e.Usuario)
            .Include(e => e.Area)
            .Where(e => e.IdArea == idArea && e.Activo)
            .ToListAsync();
    }

    public async Task<bool> EsEncargadoDeAreaAsync(int idUsuario, int idArea)
    {
        return await _context.Encargados
            .AnyAsync(e => e.IdUsuario == idUsuario && e.IdArea == idArea && e.Activo);
    }

    public async Task<Encargado> AddAsync(Encargado encargado)
    {
        _context.Encargados.Add(encargado);
        await _context.SaveChangesAsync();
        return encargado;
    }

    public async Task UpdateAsync(Encargado encargado)
    {
        _context.Encargados.Update(encargado);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var encargado = await GetByIdAsync(id);
        if (encargado != null)
        {
            _context.Encargados.Remove(encargado);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteEncargadoAsync(int idUsuario, int idArea)
    {
        return await _context.Encargados
            .AnyAsync(e => e.IdUsuario == idUsuario && e.IdArea == idArea);
    }
}
