using SistemaTikets.Application.DTOs.Encargados;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;

namespace SistemaTikets.Application.Services;

public interface IEncargadoService
{
    Task<IEnumerable<EncargadoDto>> GetAllAsync();
    Task<EncargadoDto?> GetByIdAsync(int id);
    Task<IEnumerable<EncargadoDto>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<EncargadoDto>> GetByAreaAsync(int idArea);
    Task<IEnumerable<EncargadoDto>> GetEncargadosActivosPorAreaAsync(int idArea);
    Task<IEnumerable<GestorDisponibleDto>> GetGestoresDisponiblesParaAsignarAsync(int idArea, int idEncargado);
    Task<bool> EsEncargadoDeAreaAsync(int idUsuario, int idArea);
    Task<EncargadoDto> CreateAsync(CreateEncargadoRequest request);
    Task<EncargadoDto> UpdateAsync(int id, UpdateEncargadoRequest request);
    Task DeleteAsync(int id);
}

public class EncargadoService : IEncargadoService
{
    private readonly IEncargadoRepository _encargadoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EncargadoService(
        IEncargadoRepository encargadoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _encargadoRepository = encargadoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<EncargadoDto>> GetAllAsync()
    {
        var encargados = await _encargadoRepository.GetAllAsync();
        return MapToDto(encargados);
    }

    public async Task<EncargadoDto?> GetByIdAsync(int id)
    {
        var encargado = await _encargadoRepository.GetByIdAsync(id);
        return encargado == null ? null : MapToDto(new[] { encargado }).First();
    }

    public async Task<IEnumerable<EncargadoDto>> GetByUsuarioAsync(int idUsuario)
    {
        var encargados = await _encargadoRepository.GetByUsuarioAsync(idUsuario);
        return MapToDto(encargados);
    }

    public async Task<IEnumerable<EncargadoDto>> GetByAreaAsync(int idArea)
    {
        var encargados = await _encargadoRepository.GetByAreaAsync(idArea);
        return MapToDto(encargados);
    }

    public async Task<IEnumerable<EncargadoDto>> GetEncargadosActivosPorAreaAsync(int idArea)
    {
        var encargados = await _encargadoRepository.GetEncargadosActivosPorAreaAsync(idArea);
        return MapToDto(encargados);
    }

    public async Task<IEnumerable<GestorDisponibleDto>> GetGestoresDisponiblesParaAsignarAsync(int idArea, int idEncargado)
    {
        // Obtener todos los gestores del área
        var usuarios = await _usuarioRepository.GetByAreaAsync(idArea);
        
        // Filtrar solo gestores (rol id 2, asumiendo que es el rol Gestor)
        var gestores = usuarios.Where(u => u.IdRol == 2).ToList();
        
        // Obtener encargados activos del área
        var encargados = await _encargadoRepository.GetEncargadosActivosPorAreaAsync(idArea);
        var idsEncargados = encargados.Select(e => e.IdUsuario).ToHashSet();
        
        // Mapear a DTO con indicador de encargado
        return gestores.Select(g => new GestorDisponibleDto
        {
            IdUsuario = g.IdUsuario,
            NombreCompleto = g.NombreCompleto,
            Username = g.Username,
            EsEncargado = idsEncargados.Contains(g.IdUsuario)
        });
    }

    public async Task<bool> EsEncargadoDeAreaAsync(int idUsuario, int idArea)
    {
        return await _encargadoRepository.EsEncargadoDeAreaAsync(idUsuario, idArea);
    }

    public async Task<EncargadoDto> CreateAsync(CreateEncargadoRequest request)
    {
        // Validar que el usuario existe y es gestor
        var usuario = await _usuarioRepository.GetByIdAsync(request.IdUsuario);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");
        
        if (usuario.IdRol != 2) // Asumiendo que 2 es el rol Gestor
            throw new InvalidOperationException("Solo los gestores pueden ser encargados");
        
        // Validar que el usuario pertenece al área
        if (usuario.IdAreaAsignada != request.IdArea)
            throw new InvalidOperationException("El usuario no pertenece al área especificada");
        
        // Validar que no existe un encargado duplicado
        if (await _encargadoRepository.ExisteEncargadoAsync(request.IdUsuario, request.IdArea))
            throw new InvalidOperationException("El usuario ya es encargado de esta área");
        
        var encargado = new Encargado
        {
            IdUsuario = request.IdUsuario,
            IdArea = request.IdArea,
            Activo = true,
            FechaAsignacion = DateTime.UtcNow
        };
        
        var created = await _encargadoRepository.AddAsync(encargado);
        return MapToDto(new[] { created }).First();
    }

    public async Task<EncargadoDto> UpdateAsync(int id, UpdateEncargadoRequest request)
    {
        var encargado = await _encargadoRepository.GetByIdAsync(id);
        if (encargado == null)
            throw new InvalidOperationException("Encargado no encontrado");
        
        encargado.Activo = request.Activo;
        
        await _encargadoRepository.UpdateAsync(encargado);
        return MapToDto(new[] { encargado }).First();
    }

    public async Task DeleteAsync(int id)
    {
        await _encargadoRepository.DeleteAsync(id);
    }

    private static IEnumerable<EncargadoDto> MapToDto(IEnumerable<Encargado> encargados)
    {
        return encargados.Select(e => new EncargadoDto
        {
            IdEncargado = e.IdEncargado,
            IdUsuario = e.IdUsuario,
            NombreUsuario = e.Usuario.NombreCompleto,
            IdArea = e.IdArea,
            NombreArea = e.Area.Nombre,
            FechaAsignacion = e.FechaAsignacion,
            Activo = e.Activo
        });
    }
}
