using Microsoft.EntityFrameworkCore;

using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Repositories;

public class ComentarioRepository : IComentarioRepository
{
    private readonly ApplicationDbContext _context;

    public ComentarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Comentario>> GetBySolicitudAsync(int idSolicitud)
    {
        return await _context.Comentarios
            .Include(c => c.Usuario)
                .ThenInclude(u => u.Rol)
            .Include(c => c.Usuario)
                .ThenInclude(u => u.EncargadosDeAreas)
            .Where(c => c.IdSolicitud == idSolicitud)
            .OrderBy(c => c.FechaComentario)
            .ToListAsync();
    }

    public async Task<Comentario> AddAsync(Comentario comentario)
    {
        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();
        return comentario;
    }

    public async Task DeleteAsync(int id)
    {
        var comentario = await _context.Comentarios.FindAsync(id);
        if (comentario != null)
        {
            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();
        }
    }
}
