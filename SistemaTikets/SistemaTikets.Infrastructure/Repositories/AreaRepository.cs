using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;


namespace SistemaTikets.Infrastructure.Repositories;

public class AreaRepository : IAreaRepository
{
    private readonly ApplicationDbContext _context;

    public AreaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Area?> GetByIdAsync(int id)
    {
        return await _context.Areas
            .Include(a => a.TiposSolicitud)
            .FirstOrDefaultAsync(a => a.IdArea == id);
    }

    public async Task<IEnumerable<Area>> GetAllAsync()
    {
        return await _context.Areas
            .Include(a => a.TiposSolicitud)
            .ToListAsync();
    }

    public async Task<Area> AddAsync(Area area)
    {
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();
        return area;
    }

    public async Task UpdateAsync(Area area)
    {
        _context.Areas.Update(area);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var area = await _context.Areas.FindAsync(id);
        if (area != null)
        {
            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
        }
    }
}
