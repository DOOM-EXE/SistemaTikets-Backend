using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class EstadoRepository : IEstadoRepository
{
    private readonly ApplicationDbContext _context;

    public EstadoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Estado?> GetByIdAsync(int id)
    {
        return await _context.Estados.FindAsync(id);
    }

    public async Task<IEnumerable<Estado>> GetAllAsync()
    {
        return await _context.Estados.ToListAsync();
    }

    public async Task<Estado> AddAsync(Estado estado)
    {
        _context.Estados.Add(estado);
        await _context.SaveChangesAsync();
        return estado;
    }

    public async Task UpdateAsync(Estado estado)
    {
        _context.Estados.Update(estado);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var estado = await _context.Estados.FindAsync(id);
        if (estado != null)
        {
            _context.Estados.Remove(estado);
            await _context.SaveChangesAsync();
        }
    }
}
