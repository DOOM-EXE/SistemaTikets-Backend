using SistemaTikets.Application.DTOs.Auth;
using SistemaTikets.Domain.Interfaces;

namespace SistemaTikets.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, string? ipAddress = null);
}

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly Func<string, string, string, int, string> _generateToken;
    private readonly IAuditoriaService? _auditoriaService;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        Func<string, string, string, int, string> generateToken,
        IAuditoriaService? auditoriaService = null)
    {
        _usuarioRepository = usuarioRepository;
        _generateToken = generateToken;
        _auditoriaService = auditoriaService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, string? ipAddress = null)
    {
        var usuario = await _usuarioRepository.GetByUsernameAsync(request.Username);

        if (usuario == null)
        {
            // Usuario no existe
            try
            {
                if (_auditoriaService != null)
                {
                    await _auditoriaService.RegistrarLoginFallido(request.Username, ipAddress);
                }
            }
            catch (Exception)
            {
                // Ignorar errores de auditoría para no bloquear el login
            }
            return null;
        }

        var passwordHash = HashPassword(request.Password);
        if (usuario.PasswordHash != passwordHash)
        {
            // Contraseña incorrecta
            try
            {
                if (_auditoriaService != null)
                {
                    await _auditoriaService.RegistrarLoginFallido(request.Username, ipAddress);
                }
            }
            catch (Exception)
            {
                // Ignorar errores de auditoría
            }
            return null;
        }

        if (!usuario.Activo)
        {
            // Usuario inactivo
            try
            {
                if (_auditoriaService != null)
                {
                    await _auditoriaService.RegistrarLoginFallido(request.Username, ipAddress);
                }
            }
            catch (Exception)
            {
                // Ignorar errores de auditoría
            }
            return null;
        }

        var token = _generateToken(
            usuario.IdUsuario.ToString(),
            usuario.Username,
            usuario.Rol.Nombre,
            usuario.IdAreaAsignada ?? 0);

        // Login exitoso
        try
        {
            if (_auditoriaService != null)
            {
                await _auditoriaService.RegistrarLoginExitoso(usuario.IdUsuario, usuario.Username, ipAddress);
            }
        }
        catch (Exception)
        {
        }

        return new LoginResponse
        {
            Token = token,
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Username = usuario.Username,
            Rol = usuario.Rol.Nombre,
            IdAreaAsignada = usuario.IdAreaAsignada,
            DebeCambiarPassword = usuario.DebeCambiarPassword
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
