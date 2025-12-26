using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class RolRepository : IRolRepository
{
    private readonly ApplicationDbContext _context;

    public RolRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rol?> GetByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<IEnumerable<Rol>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Rol> AddAsync(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();
        return rol;
    }

    public async Task UpdateAsync(Rol rol)
    {
        _context.Roles.Update(rol);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol != null)
        {
            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
        }
    }
}
