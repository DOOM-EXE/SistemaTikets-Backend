using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class PrioridadRepository : IPrioridadRepository
{
    private readonly ApplicationDbContext _context;

    public PrioridadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Prioridad?> GetByIdAsync(int id)
    {
        return await _context.Prioridades.FindAsync(id);
    }

    public async Task<IEnumerable<Prioridad>> GetAllAsync()
    {
        return await _context.Prioridades.ToListAsync();
    }

    public async Task<Prioridad> AddAsync(Prioridad prioridad)
    {
        _context.Prioridades.Add(prioridad);
        await _context.SaveChangesAsync();
        return prioridad;
    }

    public async Task UpdateAsync(Prioridad prioridad)
    {
        _context.Prioridades.Update(prioridad);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var prioridad = await _context.Prioridades.FindAsync(id);
        if (prioridad != null)
        {
            _context.Prioridades.Remove(prioridad);
            await _context.SaveChangesAsync();
        }
    }
}
