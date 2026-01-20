using SistemaTikets.Application.DTOs.Usuarios;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;

namespace SistemaTikets.Application.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request, int idCreador, string? ipAddress = null);
    Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioRequest request, int idEditor, string? ipAddress = null);
    Task<UsuarioDto> CambiarEstadoAsync(int id, bool activo, int idEditor, string? ipAddress = null);
}

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IAreaRepository _areaRepository;
    private readonly IAuditoriaService? _auditoriaService;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IAreaRepository areaRepository,
        IAuditoriaService? auditoriaService = null)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _areaRepository = areaRepository;
        _auditoriaService = auditoriaService;
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

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request, int idCreador, string? ipAddress = null)
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
        
        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarCreacionUsuario(idCreador, created, ipAddress);
        }

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

    public async Task<UsuarioDto> UpdateAsync(int id, UpdateUsuarioRequest request, int idEditor, string? ipAddress = null)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        var valoresAnteriores = new
        {
            usuario.NombreCompleto,
            usuario.Username,
            usuario.IdRol,
            usuario.IdAreaAsignada
        };

        bool cambioPassword = false;

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
            cambioPassword = true;
            if (request.CambiarSoloPassword == true)
            {
                usuario.DebeCambiarPassword = false;
            }
        }

        // Si el admin está editando a otro usuario y envía el flag, lo actualiza
        if (request.DebeCambiarPassword.HasValue)
        {
            // Solo permitir que el admin fuerce el cambio para otros usuarios
            if (idEditor != id)
            {
                var editor = await _usuarioRepository.GetByIdAsync(idEditor);
                if (editor != null && editor.Rol != null && editor.Rol.Nombre == "Admin")
                {
                    usuario.DebeCambiarPassword = request.DebeCambiarPassword.Value;
                }
            }
        }

        await _usuarioRepository.UpdateAsync(usuario);

        var valoresNuevos = new
        {
            usuario.NombreCompleto,
            usuario.Username,
            usuario.IdRol,
            usuario.IdAreaAsignada
        };

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEdicionUsuario(idEditor, id, valoresAnteriores, valoresNuevos, ipAddress);
            
            // Registrar cambio de password por separado si aplica
            if (cambioPassword)
            {
                await _auditoriaService.RegistrarCambioPasswordUsuario(idEditor, id, ipAddress);
            }
        }

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

    public async Task<UsuarioDto> CambiarEstadoAsync(int id, bool activo, int idEditor, string? ipAddress = null)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        var estadoAnterior = usuario.Activo;
        usuario.Activo = activo;
        await _usuarioRepository.UpdateAsync(usuario);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarCambioEstadoUsuario(idEditor, id, estadoAnterior, activo, ipAddress);
        }

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
