using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;
using System.Text.Json;

namespace SistemaTikets.Application.Services;

public interface IAuditoriaService
{
    Task RegistrarLoginExitoso(int idUsuario, string username, string? ip);
    Task RegistrarLoginFallido(string username, string? ip);
    Task RegistrarCreacionUsuario(int idUsuarioCreador, Usuario usuarioCreado, string? ip);
    Task RegistrarEdicionUsuario(int idUsuarioEditor, int idUsuarioEditado, object valoresAnteriores, object valoresNuevos, string? ip);
    Task RegistrarCambioPasswordUsuario(int idUsuarioEditor, int idUsuarioEditado, string? ip);
    Task RegistrarCambioEstadoUsuario(int idUsuarioEditor, int idUsuarioAfectado, bool estadoAnterior, bool estadoNuevo, string? ip);
    Task RegistrarCreacionSolicitud(int idUsuarioCreador, Solicitud solicitud, string? ip);
    Task RegistrarEdicionSolicitud(int idUsuarioEditor, int idSolicitud, object valoresAnteriores, object valoresNuevos, string? ip);
    Task RegistrarCambioEstadoSolicitud(int idUsuarioActor, int idSolicitud, string estadoAnterior, string estadoNuevo, string? ip);
    Task RegistrarAsignacionGestor(int idUsuarioAsignador, int idSolicitud, int? gestorAnterior, int gestorNuevo, string? ip);
    Task RegistrarTomarSolicitud(int idGestor, int idSolicitud, string nombreGestor, string? ip);
    Task RegistrarCreacionComentario(int idUsuario, int idSolicitud, string comentario, string? ip);
    Task RegistrarSubidaArchivo(int idUsuario, string nombreArchivo, long tamano, string? ip);
    Task RegistrarDescargaArchivo(int idUsuario, string nombreArchivo, string? ip);
    Task RegistrarCreacionCatalogo(int idUsuario, string tipoCatalogo, int idEntidad, string nombre, string? ip);
    Task RegistrarEdicionCatalogo(int idUsuario, string tipoCatalogo, int idEntidad, string nombreAnterior, string nombreNuevo, string? ip);
    Task RegistrarEliminacionCatalogo(int idUsuario, string tipoCatalogo, int idEntidad, string nombre, string? ip);
    Task RegistrarError(int? idUsuario, string entidad, string accion, string descripcionError, string? ip);
}

public class AuditoriaService : IAuditoriaService
{
    private readonly ILogAuditoriaRepository _logRepository;

    public AuditoriaService(ILogAuditoriaRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task RegistrarLoginExitoso(int idUsuario, string username, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = "Login",
            EntidadAfectada = "Autenticacion",
            IdEntidad = idUsuario,
            IpOrigen = ip,
            Descripcion = $"Usuario '{username}' inicio sesion exitosamente",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarLoginFallido(string username, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = null,
            TipoAccion = "Login",
            EntidadAfectada = "Autenticacion",
            IpOrigen = ip,
            Descripcion = $"Intento de inicio de sesion fallido para usuario '{username}'",
            Resultado = "Error"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarCreacionUsuario(int idUsuarioCreador, Usuario usuarioCreado, string? ip)
    {
        var valoresNuevos = new
        {
            usuarioCreado.IdUsuario,
            usuarioCreado.Username,
            usuarioCreado.NombreCompleto,
            usuarioCreado.IdRol,
            usuarioCreado.IdAreaAsignada
        };

        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioCreador,
            TipoAccion = "Crear",
            EntidadAfectada = "Usuario",
            IdEntidad = usuarioCreado.IdUsuario,
            ValoresNuevos = JsonSerializer.Serialize(valoresNuevos),
            IpOrigen = ip,
            Descripcion = $"Usuario '{usuarioCreado.Username}' creado exitosamente",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarEdicionUsuario(int idUsuarioEditor, int idUsuarioEditado, object valoresAnteriores, object valoresNuevos, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioEditor,
            TipoAccion = "Editar",
            EntidadAfectada = "Usuario",
            IdEntidad = idUsuarioEditado,
            ValoresAnteriores = JsonSerializer.Serialize(valoresAnteriores),
            ValoresNuevos = JsonSerializer.Serialize(valoresNuevos),
            IpOrigen = ip,
            Descripcion = $"Usuario ID {idUsuarioEditado} editado",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarCambioPasswordUsuario(int idUsuarioEditor, int idUsuarioEditado, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioEditor,
            TipoAccion = "CambiarPassword",
            EntidadAfectada = "Usuario",
            IdEntidad = idUsuarioEditado,
            IpOrigen = ip,
            Descripcion = $"Se cambió la contraseña del usuario ID {idUsuarioEditado}",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarCambioEstadoUsuario(int idUsuarioEditor, int idUsuarioAfectado, bool estadoAnterior, bool estadoNovo, string? ip)
    {
        var accion = estadoNovo ? "Activar" : "Desactivar";
        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioEditor,
            TipoAccion = "CambiarEstado",
            EntidadAfectada = "Usuario",
            IdEntidad = idUsuarioAfectado,
            ValoresAnteriores = JsonSerializer.Serialize(new { Activo = estadoAnterior }),
            ValoresNuevos = JsonSerializer.Serialize(new { Activo = estadoNovo }),
            IpOrigen = ip,
            Descripcion = $"Usuario ID {idUsuarioAfectado} {(estadoNovo ? "activado" : "desactivado")}",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarCreacionSolicitud(int idUsuarioCreador, Solicitud solicitud, string? ip)
    {
        var valoresNuevos = new
        {
            solicitud.IdSolicitud,
            solicitud.Codigo,
            solicitud.Asunto,
            solicitud.IdArea,
            solicitud.IdTipoSolicitud,
            solicitud.IdPrioridad,
            solicitud.IdEstado
        };

        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioCreador,
            TipoAccion = "Crear",
            EntidadAfectada = "Solicitud",
            IdEntidad = solicitud.IdSolicitud,
            ValoresNuevos = JsonSerializer.Serialize(valoresNuevos),
            IpOrigen = ip,
            Descripcion = $"Solicitud '{solicitud.Codigo}' creada: {solicitud.Asunto}",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarEdicionSolicitud(int idUsuarioEditor, int idSolicitud, object valoresAnteriores, object valoresNuevos, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioEditor,
            TipoAccion = "Editar",
            EntidadAfectada = "Solicitud",
            IdEntidad = idSolicitud,
            ValoresAnteriores = JsonSerializer.Serialize(valoresAnteriores),
            ValoresNuevos = JsonSerializer.Serialize(valoresNuevos),
            IpOrigen = ip,
            Descripcion = $"Solicitud ID {idSolicitud} editada",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarCambioEstadoSolicitud(int idUsuarioActor, int idSolicitud, string estadoAnterior, string estadoNuevo, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioActor,
            TipoAccion = "CambiarEstado",
            EntidadAfectada = "Solicitud",
            IdEntidad = idSolicitud,
            ValoresAnteriores = JsonSerializer.Serialize(new { Estado = estadoAnterior }),
            ValoresNuevos = JsonSerializer.Serialize(new { Estado = estadoNuevo }),
            IpOrigen = ip,
            Descripcion = $"Solicitud ID {idSolicitud} cambio de estado: {estadoAnterior} -> {estadoNuevo}",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarAsignacionGestor(int idUsuarioAsignador, int idSolicitud, int? gestorAnterior, int gestorNuevo, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuarioAsignador,
            TipoAccion = "AsignarGestor",
            EntidadAfectada = "Solicitud",
            IdEntidad = idSolicitud,
            ValoresAnteriores = gestorAnterior.HasValue ? JsonSerializer.Serialize(new { IdGestor = gestorAnterior.Value }) : null,
            ValoresNuevos = JsonSerializer.Serialize(new { IdGestor = gestorNuevo }),
            IpOrigen = ip,
            Descripcion = $"Solicitud ID {idSolicitud} asignada a gestor ID {gestorNuevo}",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarTomarSolicitud(int idGestor, int idSolicitud, string nombreGestor, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idGestor,
            TipoAccion = "TomarSolicitud",
            EntidadAfectada = "Solicitud",
            IdEntidad = idSolicitud,
            ValoresNuevos = JsonSerializer.Serialize(new { IdGestor = idGestor, NombreGestor = nombreGestor }),
            IpOrigen = ip,
            Descripcion = $"El gestor {nombreGestor} tomó la solicitud ID {idSolicitud}",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarCreacionComentario(int idUsuario, int idSolicitud, string comentario, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = "Crear",
            EntidadAfectada = "Comentario",
            IdEntidad = idSolicitud,
            ValoresNuevos = JsonSerializer.Serialize(new { Comentario = comentario.Length > 100 ? comentario.Substring(0, 100) + "..." : comentario }),
            IpOrigen = ip,
            Descripcion = $"Comentario agregado a solicitud ID {idSolicitud}",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarSubidaArchivo(int idUsuario, string nombreArchivo, long tamano, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = "SubirArchivo",
            EntidadAfectada = "Archivo",
            ValoresNuevos = JsonSerializer.Serialize(new { NombreArchivo = nombreArchivo, Tamano = tamano }),
            IpOrigen = ip,
            Descripcion = $"Archivo '{nombreArchivo}' subido ({tamano} bytes)",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarDescargaArchivo(int idUsuario, string nombreArchivo, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = "DescargarArchivo",
            EntidadAfectada = "Archivo",
            ValoresNuevos = JsonSerializer.Serialize(new { NombreArchivo = nombreArchivo }),
            IpOrigen = ip,
            Descripcion = $"Archivo '{nombreArchivo}' descargado",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarCreacionCatalogo(int idUsuario, string tipoCatalogo, int idEntidad, string nombre, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = "Crear",
            EntidadAfectada = tipoCatalogo,
            IdEntidad = idEntidad,
            ValoresNuevos = JsonSerializer.Serialize(new { Nombre = nombre }),
            IpOrigen = ip,
            Descripcion = $"{tipoCatalogo} '{nombre}' creado exitosamente",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarEdicionCatalogo(int idUsuario, string tipoCatalogo, int idEntidad, string nombreAnterior, string nombreNuevo, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = "Editar",
            EntidadAfectada = tipoCatalogo,
            IdEntidad = idEntidad,
            ValoresAnteriores = JsonSerializer.Serialize(new { Nombre = nombreAnterior }),
            ValoresNuevos = JsonSerializer.Serialize(new { Nombre = nombreNuevo }),
            IpOrigen = ip,
            Descripcion = $"{tipoCatalogo} ID {idEntidad} editado: '{nombreAnterior}' -> '{nombreNuevo}'",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarEliminacionCatalogo(int idUsuario, string tipoCatalogo, int idEntidad, string nombre, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = "Eliminar",
            EntidadAfectada = tipoCatalogo,
            IdEntidad = idEntidad,
            ValoresAnteriores = JsonSerializer.Serialize(new { Nombre = nombre }),
            IpOrigen = ip,
            Descripcion = $"{tipoCatalogo} '{nombre}' eliminado",
            Resultado = "Exitoso"
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task RegistrarError(int? idUsuario, string entidad, string accion, string descripcionError, string? ip)
    {
        var log = new LogAuditoria
        {
            IdUsuario = idUsuario,
            TipoAccion = accion,
            EntidadAfectada = entidad,
            IpOrigen = ip,
            Descripcion = descripcionError,
            Resultado = "Error"
        };

        await _logRepository.CreateAsync(log);
    }
}
