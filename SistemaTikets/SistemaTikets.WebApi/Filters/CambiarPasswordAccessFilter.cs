using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaTikets.Domain.Interfaces;

namespace SistemaTikets.WebApi.Filters;

public class CambiarPasswordAccessFilter : IAsyncActionFilter
{
    private readonly IUsuarioRepository _usuarioRepository;

    public CambiarPasswordAccessFilter(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "No autenticado" });
            return;
        }

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(idClaim))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "No autenticado" });
            return;
        }

        int idUsuario = int.Parse(idClaim);
        int idTarget = 0;
        if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is int)
        {
            idTarget = (int)idObj;
        }
        else
        {
            context.Result = new BadRequestObjectResult(new { message = "Id de usuario no válido" });
            return;
        }

        // Permitir si es admin
        if (roleClaim == "Admin")
        {
            await next();
            return;
        }

        // Permitir si el usuario es el mismo y debe cambiar password
        if (idUsuario == idTarget)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
            if (usuario != null && usuario.DebeCambiarPassword)
            {
                await next();
                return;
            }
            else if (usuario != null && !usuario.DebeCambiarPassword)
            {
                context.Result = new ForbidResult("No autorizado: Solo puede cambiar la contraseña si el sistema lo requiere.");
                return;
            }
        }

        context.Result = new ForbidResult("No autorizado para cambiar la contraseña de este usuario.");
    }
}
