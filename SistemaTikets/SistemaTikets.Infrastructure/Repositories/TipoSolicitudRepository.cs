using Microsoft.EntityFrameworkCore;

using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class TipoSolicitudRepository : ITipoSolicitudRepository
{
    private readonly ApplicationDbContext _context;

    public TipoSolicitudRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TipoSolicitud?> GetByIdAsync(int id)
    {
        return await _context.TiposSolicitud
            .Include(t => t.Area)
            .FirstOrDefaultAsync(t => t.IdTipoSolicitud == id);
    }

    public async Task<IEnumerable<TipoSolicitud>> GetAllAsync()
    {
        return await _context.TiposSolicitud
            .Include(t => t.Area)
            .ToListAsync();
    }

    public async Task<IEnumerable<TipoSolicitud>> GetByAreaAsync(int idArea)
    {
        return await _context.TiposSolicitud
            .Where(t => t.IdArea == idArea)
            .ToListAsync();
    }

    public async Task<TipoSolicitud> AddAsync(TipoSolicitud tipoSolicitud)
    {
        _context.TiposSolicitud.Add(tipoSolicitud);
        await _context.SaveChangesAsync();
        return tipoSolicitud;
    }

    public async Task UpdateAsync(TipoSolicitud tipoSolicitud)
    {
        _context.TiposSolicitud.Update(tipoSolicitud);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tipoSolicitud = await _context.TiposSolicitud.FindAsync(id);
        if (tipoSolicitud != null)
        {
            _context.TiposSolicitud.Remove(tipoSolicitud);
            await _context.SaveChangesAsync();
        }
    }
}
