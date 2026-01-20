using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class LogAuditoriaRepository : ILogAuditoriaRepository
{
    private readonly ApplicationDbContext _context;

    public LogAuditoriaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LogAuditoria> CreateAsync(LogAuditoria log)
    {
        await _context.LogsAuditoria.AddAsync(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<IEnumerable<LogAuditoria>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
    {
        return await _context.LogsAuditoria
            .Include(l => l.Usuario)
            .OrderByDescending(l => l.FechaEvento)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAuditoria>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.LogsAuditoria
            .Include(l => l.Usuario)
            .Where(l => l.IdUsuario == idUsuario)
            .OrderByDescending(l => l.FechaEvento)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAuditoria>> GetByEntidadAsync(string entidad, int? idEntidad = null)
    {
        var query = _context.LogsAuditoria
            .Include(l => l.Usuario)
            .Where(l => l.EntidadAfectada == entidad);

        if (idEntidad.HasValue)
        {
            query = query.Where(l => l.IdEntidad == idEntidad.Value);
        }

        return await query
            .OrderByDescending(l => l.FechaEvento)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAuditoria>> GetByFechasAsync(DateTime fechaInicio, DateTime fechaFin, int pageNumber = 1, int pageSize = 50)
    {
        return await _context.LogsAuditoria
            .Include(l => l.Usuario)
            .Where(l => l.FechaEvento >= fechaInicio && l.FechaEvento <= fechaFin)
            .OrderByDescending(l => l.FechaEvento)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAuditoria>> GetByTipoAccionAsync(string tipoAccion)
    {
        return await _context.LogsAuditoria
            .Include(l => l.Usuario)
            .Where(l => l.TipoAccion == tipoAccion)
            .OrderByDescending(l => l.FechaEvento)
            .ToListAsync();
    }

    public async Task<LogAuditoria?> GetByIdAsync(int id)
    {
        return await _context.LogsAuditoria
            .Include(l => l.Usuario)
            .FirstOrDefaultAsync(l => l.IdLog == id);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.LogsAuditoria.CountAsync();
    }

    public async Task<IEnumerable<LogAuditoria>> GetByFiltrosAsync(
        int? idUsuario,
        string? tipoAccion,
        string? entidadAfectada,
        int? idEntidad,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? resultado,
        int pageNumber = 1,
        int pageSize = 50
    )
    {
        var query = _context.LogsAuditoria.Include(l => l.Usuario).AsQueryable();

        if (idUsuario.HasValue)
            query = query.Where(l => l.IdUsuario == idUsuario.Value);
        if (!string.IsNullOrEmpty(tipoAccion))
            query = query.Where(l => l.TipoAccion == tipoAccion);
        if (!string.IsNullOrEmpty(entidadAfectada))
            query = query.Where(l => l.EntidadAfectada == entidadAfectada);
        if (idEntidad.HasValue)
            query = query.Where(l => l.IdEntidad == idEntidad.Value);
        if (fechaInicio.HasValue)
            query = query.Where(l => l.FechaEvento >= fechaInicio.Value);
        if (fechaFin.HasValue)
            query = query.Where(l => l.FechaEvento <= fechaFin.Value);
        if (!string.IsNullOrEmpty(resultado))
            query = query.Where(l => l.Resultado == resultado);

        return await query
            .OrderByDescending(l => l.FechaEvento)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountByFiltrosAsync(
        int? idUsuario,
        string? tipoAccion,
        string? entidadAfectada,
        int? idEntidad,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? resultado
    )
    {
        var query = _context.LogsAuditoria.AsQueryable();

        if (idUsuario.HasValue)
            query = query.Where(l => l.IdUsuario == idUsuario.Value);
        if (!string.IsNullOrEmpty(tipoAccion))
            query = query.Where(l => l.TipoAccion == tipoAccion);
        if (!string.IsNullOrEmpty(entidadAfectada))
            query = query.Where(l => l.EntidadAfectada == entidadAfectada);
        if (idEntidad.HasValue)
            query = query.Where(l => l.IdEntidad == idEntidad.Value);
        if (fechaInicio.HasValue)
            query = query.Where(l => l.FechaEvento >= fechaInicio.Value);
        if (fechaFin.HasValue)
            query = query.Where(l => l.FechaEvento <= fechaFin.Value);
        if (!string.IsNullOrEmpty(resultado))
            query = query.Where(l => l.Resultado == resultado);

        return await query.CountAsync();
    }
}
