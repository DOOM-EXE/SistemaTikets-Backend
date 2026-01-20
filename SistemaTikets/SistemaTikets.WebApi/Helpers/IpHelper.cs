using Microsoft.AspNetCore.Http;

namespace SistemaTikets.WebApi.Helpers;

public static class IpHelper
{
    public static string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null)
            return null;

        var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        }

        if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Contains(","))
        {
            ipAddress = ipAddress.Split(',')[0].Trim();
        }

        return ipAddress;
    }
}
