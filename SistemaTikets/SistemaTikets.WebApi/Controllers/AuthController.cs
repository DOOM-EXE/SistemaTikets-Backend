using Microsoft.AspNetCore.Mvc;
using SistemaTikets.Application.DTOs.Auth;
using SistemaTikets.Application.Services;
using SistemaTikets.WebApi.Helpers;

namespace SistemaTikets.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
        var response = await _authService.LoginAsync(request, ipAddress);

        if (response == null)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        return Ok(response);
    }
}
