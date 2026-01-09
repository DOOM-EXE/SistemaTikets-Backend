using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class SolicitudRepository : ISolicitudRepository
{
    private readonly ApplicationDbContext _context;

    public SolicitudRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Solicitud?> GetByIdAsync(int id)
    {
        return await _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Include(s => s.GestorAsignado)
            .Include(s => s.AsignadoPor)
            .Include(s => s.Trazabilidades.OrderBy(t => t.FechaEvento))
            .Include(s => s.Comentarios.OrderBy(c => c.FechaComentario))
            .FirstOrDefaultAsync(s => s.IdSolicitud == id);
    }

    public async Task<Solicitud?> GetByCodigoAsync(string codigo)
    {
        return await _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Include(s => s.GestorAsignado)
            .Include(s => s.AsignadoPor)
            .FirstOrDefaultAsync(s => s.Codigo == codigo);
    }

    public async Task<IEnumerable<Solicitud>> GetAllAsync()
    {
        return await _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Include(s => s.GestorAsignado)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetByFiltrosAsync(
        int? idEstado = null,
        int? idArea = null,
        int? idPrioridad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        string? busqueda = null)
    {
        var query = _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Include(s => s.GestorAsignado)
            .AsQueryable();

        if (idEstado.HasValue)
            query = query.Where(s => s.IdEstado == idEstado.Value);

        if (idArea.HasValue)
            query = query.Where(s => s.IdArea == idArea.Value);

        if (idPrioridad.HasValue)
            query = query.Where(s => s.IdPrioridad == idPrioridad.Value);

        if (fechaDesde.HasValue)
            query = query.Where(s => s.FechaCreacion >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(s => s.FechaCreacion <= fechaHasta.Value);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            query = query.Where(s =>
                (s.Codigo != null && s.Codigo.Contains(busqueda)) ||
                s.Asunto.Contains(busqueda) ||
                s.Descripcion.Contains(busqueda));
        }

        return await query.OrderByDescending(s => s.FechaCreacion).ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetBySolicitanteAsync(int idSolicitante)
    {
        return await _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Include(s => s.GestorAsignado)
            .Where(s => s.IdSolicitante == idSolicitante)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetByGestorAsync(int idGestor)
    {
        return await _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Where(s => s.IdGestorAsignado == idGestor)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetByAreaAsync(int idArea)
    {
        return await _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Include(s => s.GestorAsignado)
            .Where(s => s.IdArea == idArea)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Solicitud> AddAsync(Solicitud solicitud)
    {
        _context.Solicitudes.Add(solicitud);
        await _context.SaveChangesAsync();

        // Recargar con todas las relaciones de navegación
        return await _context.Solicitudes
            .Include(s => s.Solicitante)
            .Include(s => s.Area)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Estado)
            .Include(s => s.GestorAsignado)
            .FirstAsync(s => s.IdSolicitud == solicitud.IdSolicitud);
    }

    public async Task UpdateAsync(Solicitud solicitud)
    {
        _context.Solicitudes.Update(solicitud);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);
        if (solicitud != null)
        {
            _context.Solicitudes.Remove(solicitud);
            await _context.SaveChangesAsync();
        }
    }
}
