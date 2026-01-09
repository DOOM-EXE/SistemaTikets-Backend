using SistemaTikets.Application.DTOs.Auth;
using SistemaTikets.Domain.Interfaces;

namespace SistemaTikets.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly Func<string, string, string, int, string> _generateToken;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        Func<string, string, string, int, string> generateToken)
    {
        _usuarioRepository = usuarioRepository;
        _generateToken = generateToken;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.GetByUsernameAsync(request.Username);

        if (usuario == null)
            return null;

        // Verificar contraseña (usando PasswordHasher de Infrastructure)
        var passwordHash = HashPassword(request.Password);
        if (usuario.PasswordHash != passwordHash)
            return null;

        // Verificar que el usuario esté activo
        if (!usuario.Activo)
            return null;

        // Generar token JWT
        var token = _generateToken(
            usuario.IdUsuario.ToString(),
            usuario.Username,
            usuario.Rol.Nombre,
            usuario.IdAreaAsignada ?? 0);

        return new LoginResponse
        {
            Token = token,
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Username = usuario.Username,
            Rol = usuario.Rol.Nombre,
            IdAreaAsignada = usuario.IdAreaAsignada
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
