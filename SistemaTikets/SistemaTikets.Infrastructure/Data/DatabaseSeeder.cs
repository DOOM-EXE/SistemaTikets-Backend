using Microsoft.EntityFrameworkCore;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Infrastructure.Persistence;
using SistemaTikets.Infrastructure.Security;

namespace SistemaTikets.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar si ya hay datos en las tablas principales
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        await SeedRolesAsync(context);
        await SeedEstadosAsync(context);
        await SeedPrioridadesAsync(context);
        await SeedAreasAsync(context);
        await SeedTiposSolicitudAsync(context);
        await SeedUsuariosAsync(context);
        await SeedEncargadosAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        var roles = new List<Rol>
        {
            new() { Nombre = "Admin" },
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
            new() { Nombre = "En Progreso" },
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
            new() { Nombre = "Crítica" }
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

    private static async Task SeedUsuariosAsync(ApplicationDbContext context)
    {
        // Obtener roles
        var rolAdmin = await context.Roles.FirstAsync(r => r.Nombre == "Admin");
        var rolGestor = await context.Roles.FirstAsync(r => r.Nombre == "Gestor");
        var rolSolicitante = await context.Roles.FirstAsync(r => r.Nombre == "Solicitante");

        // Obtener áreas
        var areasTI = await context.Areas.FirstAsync(a => a.Nombre == "Tecnología de la Información");
        var areasRRHH = await context.Areas.FirstAsync(a => a.Nombre == "Recursos Humanos");
        var areasLogistica = await context.Areas.FirstAsync(a => a.Nombre == "Logística");
        var areasContabilidad = await context.Areas.FirstAsync(a => a.Nombre == "Contabilidad");
        var areasOperaciones = await context.Areas.FirstAsync(a => a.Nombre == "Operaciones");

        var usuarios = new List<Usuario>
        {
            // ========== ADMINISTRADORES ==========
            new()
            {
                NombreCompleto = "Super Administrador",
                Username = "superadmin",
                PasswordHash = PasswordHasher.HashPassword("SuperAdmin123!"),
                IdRol = rolAdmin.IdRol,
                IdAreaAsignada = areasTI.IdArea,
                IdCreadoPor = null,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Administrador Principal",
                Username = "admin",
                PasswordHash = PasswordHasher.HashPassword("Admin123!"),
                IdRol = rolAdmin.IdRol,
                IdAreaAsignada = areasTI.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },

            // ========== GESTORES (uno por cada área principal) ==========
            new()
            {
                NombreCompleto = "Carlos Méndez",
                Username = "gestor.ti",
                PasswordHash = PasswordHasher.HashPassword("Gestor123!"),
                IdRol = rolGestor.IdRol,
                IdAreaAsignada = areasTI.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Pedro Rodríguez",
                Username = "gestor.ti2",
                PasswordHash = PasswordHasher.HashPassword("Gestor123!"),
                IdRol = rolGestor.IdRol,
                IdAreaAsignada = areasTI.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "María González",
                Username = "gestor.rrhh",
                PasswordHash = PasswordHasher.HashPassword("Gestor123!"),
                IdRol = rolGestor.IdRol,
                IdAreaAsignada = areasRRHH.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Carmen López",
                Username = "gestor.rrhh2",
                PasswordHash = PasswordHasher.HashPassword("Gestor123!"),
                IdRol = rolGestor.IdRol,
                IdAreaAsignada = areasRRHH.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Luis Ramírez",
                Username = "gestor.logistica",
                PasswordHash = PasswordHasher.HashPassword("Gestor123!"),
                IdRol = rolGestor.IdRol,
                IdAreaAsignada = areasLogistica.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Ana Torres",
                Username = "gestor.contabilidad",
                PasswordHash = PasswordHasher.HashPassword("Gestor123!"),
                IdRol = rolGestor.IdRol,
                IdAreaAsignada = areasContabilidad.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },

            // ========== SOLICITANTES ==========
            new()
            {
                NombreCompleto = "Juan Pérez",
                Username = "jperez",
                PasswordHash = PasswordHasher.HashPassword("User123!"),
                IdRol = rolSolicitante.IdRol,
                IdAreaAsignada = areasOperaciones.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Laura Martínez",
                Username = "lmartinez",
                PasswordHash = PasswordHasher.HashPassword("User123!"),
                IdRol = rolSolicitante.IdRol,
                IdAreaAsignada = areasOperaciones.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Roberto Sánchez",
                Username = "rsanchez",
                PasswordHash = PasswordHasher.HashPassword("User123!"),
                IdRol = rolSolicitante.IdRol,
                IdAreaAsignada = areasContabilidad.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            },
            new()
            {
                NombreCompleto = "Patricia Flores",
                Username = "pflores",
                PasswordHash = PasswordHasher.HashPassword("User123!"),
                IdRol = rolSolicitante.IdRol,
                IdAreaAsignada = areasLogistica.IdArea,
                IdCreadoPor = 1,
                FechaCreacionUsuario = DateTime.UtcNow
            }
        };

        await context.Usuarios.AddRangeAsync(usuarios);
        await context.SaveChangesAsync();
    }

    private static async Task SeedEncargadosAsync(ApplicationDbContext context)
    {
        var areasTI = await context.Areas.FirstAsync(a => a.Nombre == "Tecnología de la Información");
        var areasRRHH = await context.Areas.FirstAsync(a => a.Nombre == "Recursos Humanos");
        var areasLogistica = await context.Areas.FirstAsync(a => a.Nombre == "Logística");
        var areasContabilidad = await context.Areas.FirstAsync(a => a.Nombre == "Contabilidad");

        var gestorTI1 = await context.Usuarios.FirstAsync(u => u.Username == "gestor.ti");
        var gestorTI2 = await context.Usuarios.FirstAsync(u => u.Username == "gestor.ti2");
        var gestorRRHH1 = await context.Usuarios.FirstAsync(u => u.Username == "gestor.rrhh");
        var gestorRRHH2 = await context.Usuarios.FirstAsync(u => u.Username == "gestor.rrhh2");
        var gestorLogistica = await context.Usuarios.FirstAsync(u => u.Username == "gestor.logistica");
        var gestorContabilidad = await context.Usuarios.FirstAsync(u => u.Username == "gestor.contabilidad");

        var encargados = new List<Encargado>
        {
            new()
            {
                IdUsuario = gestorTI1.IdUsuario,
                IdArea = areasTI.IdArea,
                Activo = true,
                FechaAsignacion = DateTime.UtcNow
            },
            new()
            {
                IdUsuario = gestorTI2.IdUsuario,
                IdArea = areasTI.IdArea,
                Activo = true,
                FechaAsignacion = DateTime.UtcNow
            },
            new()
            {
                IdUsuario = gestorRRHH1.IdUsuario,
                IdArea = areasRRHH.IdArea,
                Activo = true,
                FechaAsignacion = DateTime.UtcNow
            },
            new()
            {
                IdUsuario = gestorRRHH2.IdUsuario,
                IdArea = areasRRHH.IdArea,
                Activo = true,
                FechaAsignacion = DateTime.UtcNow
            },
            new()
            {
                IdUsuario = gestorLogistica.IdUsuario,
                IdArea = areasLogistica.IdArea,
                Activo = true,
                FechaAsignacion = DateTime.UtcNow
            },
            new()
            {
                IdUsuario = gestorContabilidad.IdUsuario,
                IdArea = areasContabilidad.IdArea,
                Activo = true,
                FechaAsignacion = DateTime.UtcNow
            }
        };

        await context.Encargados.AddRangeAsync(encargados);
        await context.SaveChangesAsync();
    }
}
