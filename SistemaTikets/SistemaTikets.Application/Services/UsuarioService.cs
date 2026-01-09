using SistemaTikets.Application.DTOs.Usuarios;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;

namespace SistemaTikets.Application.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request, int idCreador);
    Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioRequest request);
    Task<UsuarioDto> CambiarEstadoAsync(int id, bool activo);
}

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IAreaRepository _areaRepository;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IAreaRepository areaRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _areaRepository = areaRepository;
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return usuarios.Select(u => new UsuarioDto
        {
            IdUsuario = u.IdUsuario,
            NombreCompleto = u.NombreCompleto,
            Username = u.Username,
            IdRol = u.IdRol,
            Rol = u.Rol.Nombre,
            IdAreaAsignada = u.IdAreaAsignada,
            NombreArea = u.AreaAsignada?.Nombre,
            CreadoPor = u.CreadoPor?.NombreCompleto,
            FechaCreacionUsuario = u.FechaCreacionUsuario,
            Activo = u.Activo
        });
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            return null;

        return new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Username = usuario.Username,
            IdRol = usuario.IdRol,
            Rol = usuario.Rol.Nombre,
            IdAreaAsignada = usuario.IdAreaAsignada,
            NombreArea = usuario.AreaAsignada?.Nombre,
            CreadoPor = usuario.CreadoPor?.NombreCompleto,
            FechaCreacionUsuario = usuario.FechaCreacionUsuario,
            Activo = usuario.Activo
        };
    }

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request, int idCreador)
    {
        if (await _usuarioRepository.ExistsAsync(request.Username))
            throw new InvalidOperationException("El nombre de usuario ya existe");

        var usuario = new Usuario
        {
            NombreCompleto = request.NombreCompleto,
            Username = request.Username,
            PasswordHash = HashPassword(request.Password),
            IdRol = request.IdRol,
            IdAreaAsignada = request.IdAreaAsignada,
            IdCreadoPor = idCreador,
            FechaCreacionUsuario = DateTime.UtcNow,
            Activo = true
        };

        var created = await _usuarioRepository.AddAsync(usuario);
        var rol = await _rolRepository.GetByIdAsync(created.IdRol);
        var area = created.IdAreaAsignada.HasValue
            ? await _areaRepository.GetByIdAsync(created.IdAreaAsignada.Value)
            : null;

        return new UsuarioDto
        {
            IdUsuario = created.IdUsuario,
            NombreCompleto = created.NombreCompleto,
            Username = created.Username,
            IdRol = created.IdRol,
            Rol = rol?.Nombre ?? string.Empty,
            IdAreaAsignada = created.IdAreaAsignada,
            NombreArea = area?.Nombre,
            FechaCreacionUsuario = created.FechaCreacionUsuario,
            Activo = created.Activo
        };
    }

    public async Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioRequest request)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        usuario.NombreCompleto = request.NombreCompleto;
        
        if (!string.IsNullOrWhiteSpace(request.Username) && request.Username != usuario.Username)
        {
            if (await _usuarioRepository.ExistsAsync(request.Username))
                throw new InvalidOperationException("El nombre de usuario ya existe");
            
            usuario.Username = request.Username;
        }
        
        usuario.IdRol = request.IdRol;
        usuario.IdAreaAsignada = request.IdAreaAsignada;

        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            usuario.PasswordHash = HashPassword(request.NewPassword);
        }

        await _usuarioRepository.UpdateAsync(usuario);

        var rol = await _rolRepository.GetByIdAsync(usuario.IdRol);
        var area = usuario.IdAreaAsignada.HasValue
            ? await _areaRepository.GetByIdAsync(usuario.IdAreaAsignada.Value)
            : null;

        return new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Username = usuario.Username,
            IdRol = usuario.IdRol,
            Rol = rol?.Nombre ?? string.Empty,
            IdAreaAsignada = usuario.IdAreaAsignada,
            NombreArea = area?.Nombre,
            FechaCreacionUsuario = usuario.FechaCreacionUsuario,
            Activo = usuario.Activo
        };
    }

    public async Task<UsuarioDto> CambiarEstadoAsync(int id, bool activo)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        usuario.Activo = activo;
        await _usuarioRepository.UpdateAsync(usuario);

        var rol = await _rolRepository.GetByIdAsync(usuario.IdRol);
        var area = usuario.IdAreaAsignada.HasValue
            ? await _areaRepository.GetByIdAsync(usuario.IdAreaAsignada.Value)
            : null;

        return new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Username = usuario.Username,
            IdRol = usuario.IdRol,
            Rol = rol?.Nombre ?? string.Empty,
            IdAreaAsignada = usuario.IdAreaAsignada,
            NombreArea = area?.Nombre,
            FechaCreacionUsuario = usuario.FechaCreacionUsuario,
            Activo = usuario.Activo
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
