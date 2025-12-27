using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Infrastructure.Persistence;

namespace SistemaTikets.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar si ya hay datos en las tablas principales
        if (await context.Roles.AnyAsync())
        {
            return; // Ya hay datos, no hacer seeding
        }

        await SeedRolesAsync(context);
        await SeedEstadosAsync(context);
        await SeedPrioridadesAsync(context);
        await SeedAreasAsync(context);
        await SeedTiposSolicitudAsync(context);
        await SeedUsuarioAdminAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        var roles = new List<Rol>
        {
            new() { Nombre = "Admin" },
            new() { Nombre = "Coordinador" },
            new() { Nombre = "Gestor" },
            new() { Nombre = "Solicitante" }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedEstadosAsync(ApplicationDbContext context)
    {
        var estados = new List<Estado>
        {
            new() { Nombre = "Nueva" },
            new() { Nombre = "En Proceso" },
            new() { Nombre = "Resuelta" },
            new() { Nombre = "Cerrada" },
            new() { Nombre = "Cancelada" }
        };

        await context.Estados.AddRangeAsync(estados);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPrioridadesAsync(ApplicationDbContext context)
    {
        var prioridades = new List<Prioridad>
        {
            new() { Nombre = "Baja" },
            new() { Nombre = "Media" },
            new() { Nombre = "Alta" },
            new() { Nombre = "Urgente" }
        };

        await context.Prioridades.AddRangeAsync(prioridades);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAreasAsync(ApplicationDbContext context)
    {
        var areas = new List<Area>
        {
            new() { Nombre = "Tecnología de la Información" },
            new() { Nombre = "Recursos Humanos" },
            new() { Nombre = "Logística" },
            new() { Nombre = "Contabilidad" },
            new() { Nombre = "Operaciones" },
            new() { Nombre = "Ventas" },
            new() { Nombre = "Marketing" }
        };

        await context.Areas.AddRangeAsync(areas);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTiposSolicitudAsync(ApplicationDbContext context)
    {
        // Obtener IDs de las áreas
        var tiArea = await context.Areas.FirstAsync(a => a.Nombre == "Tecnología de la Información");
        var rrhhArea = await context.Areas.FirstAsync(a => a.Nombre == "Recursos Humanos");
        var logisticaArea = await context.Areas.FirstAsync(a => a.Nombre == "Logística");
        var contabilidadArea = await context.Areas.FirstAsync(a => a.Nombre == "Contabilidad");
        var operacionesArea = await context.Areas.FirstAsync(a => a.Nombre == "Operaciones");

        var tiposSolicitud = new List<TipoSolicitud>
        {
            // TI
            new() { Nombre = "Soporte Técnico", IdArea = tiArea.IdArea },
            new() { Nombre = "Acceso a Sistemas", IdArea = tiArea.IdArea },
            new() { Nombre = "Solicitud de Equipamiento", IdArea = tiArea.IdArea },
            new() { Nombre = "Desarrollo de Software", IdArea = tiArea.IdArea },
            new() { Nombre = "Mantenimiento de Red", IdArea = tiArea.IdArea },
            
            // RRHH
            new() { Nombre = "Solicitud de Permisos", IdArea = rrhhArea.IdArea },
            new() { Nombre = "Solicitud de Vacaciones", IdArea = rrhhArea.IdArea },
            new() { Nombre = "Capacitación", IdArea = rrhhArea.IdArea },
            new() { Nombre = "Certificados Laborales", IdArea = rrhhArea.IdArea },
            new() { Nombre = "Planilla y Beneficios", IdArea = rrhhArea.IdArea },
            
            // Logística
            new() { Nombre = "Material de Oficina", IdArea = logisticaArea.IdArea },
            new() { Nombre = "Transporte", IdArea = logisticaArea.IdArea },
            new() { Nombre = "Mantenimiento de Instalaciones", IdArea = logisticaArea.IdArea },
            new() { Nombre = "Servicios Generales", IdArea = logisticaArea.IdArea },
            
            // Contabilidad
            new() { Nombre = "Reembolso de Gastos", IdArea = contabilidadArea.IdArea },
            new() { Nombre = "Facturación", IdArea = contabilidadArea.IdArea },
            new() { Nombre = "Pago a Proveedores", IdArea = contabilidadArea.IdArea },
            
            // Operaciones
            new() { Nombre = "Mejora de Procesos", IdArea = operacionesArea.IdArea },
            new() { Nombre = "Reporte de Incidencias", IdArea = operacionesArea.IdArea },
            new() { Nombre = "Solicitud de Información", IdArea = operacionesArea.IdArea }
        };

        await context.TiposSolicitud.AddRangeAsync(tiposSolicitud);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsuarioAdminAsync(ApplicationDbContext context)
    {
        var rolAdmin = await context.Roles.FirstAsync(r => r.Nombre == "Admin");

        // Hash de la contraseña "Admin123!" usando SHA256
        var passwordHash = HashPassword("Admin123!");

        var usuarioAdmin = new Usuario
        {
            NombreCompleto = "Administrador del Sistema",
            Username = "admin",
            PasswordHash = passwordHash,
            IdRol = rolAdmin.IdRol,
            IdAreaAsignada = null,
            IdCreadoPor = null,
            FechaCreacionUsuario = DateTime.UtcNow
        };

        await context.Usuarios.AddAsync(usuarioAdmin);
        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
