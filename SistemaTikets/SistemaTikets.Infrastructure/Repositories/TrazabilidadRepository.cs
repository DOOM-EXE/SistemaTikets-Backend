using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class TrazabilidadRepository : ITrazabilidadRepository
{
    private readonly ApplicationDbContext _context;

    public TrazabilidadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrazabilidadSolicitud>> GetBySolicitudAsync(int idSolicitud)
    {
        return await _context.TrazabilidadesSolicitud
            .Include(t => t.UsuarioActor)
                .ThenInclude(u => u.Rol)
            .Include(t => t.UsuarioActor)
                .ThenInclude(u => u.EncargadosDeAreas)
            .Where(t => t.IdSolicitud == idSolicitud)
            .OrderBy(t => t.FechaEvento)
            .ToListAsync();
    }

    public async Task<TrazabilidadSolicitud> AddAsync(TrazabilidadSolicitud trazabilidad)
    {
        _context.TrazabilidadesSolicitud.Add(trazabilidad);
        await _context.SaveChangesAsync();
        return trazabilidad;
    }
}
