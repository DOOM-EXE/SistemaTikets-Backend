using SistemaTikets.Application.DTOs.Solicitudes;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;


namespace SistemaTikets.Application.Services;

public interface ISolicitudService
{
    Task<IEnumerable<SolicitudDto>> GetAllAsync();
    Task<IEnumerable<SolicitudDto>> GetByFiltrosAsync(int? idEstado, int? idArea, int? idPrioridad, DateTime? fechaDesde, DateTime? fechaHasta, string? busqueda);
    Task<IEnumerable<SolicitudDto>> GetBySolicitanteAsync(int idSolicitante);
    Task<IEnumerable<SolicitudDto>> GetByGestorAsync(int idGestor);
    Task<IEnumerable<SolicitudDto>> GetByAreaAsync(int idArea);
    Task<SolicitudDetalleDto?> GetDetalleAsync(int id);
    Task<SolicitudDto> CreateAsync(CreateSolicitudRequest request, int idSolicitante);
    Task<SolicitudDto> UpdateAsync(int id, UpdateSolicitudRequest request, int idUsuario);
    Task CambiarEstadoAsync(int id, CambiarEstadoRequest request, int idUsuario);
    Task AsignarGestorAsync(int id, AsignarGestorRequest request, int idAsignadoPor);
    Task TomarSolicitudAsync(int id, int idGestor);
    Task DeleteAsync(int id);
}

public class SolicitudService : ISolicitudService
{
    private readonly ISolicitudRepository _solicitudRepository;
    private readonly ITrazabilidadRepository _trazabilidadRepository;
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IEstadoRepository _estadoRepository;

    public SolicitudService(
        ISolicitudRepository solicitudRepository,
        ITrazabilidadRepository trazabilidadRepository,
        IComentarioRepository comentarioRepository,
        IEstadoRepository estadoRepository)
    {
        _solicitudRepository = solicitudRepository;
        _trazabilidadRepository = trazabilidadRepository;
        _comentarioRepository = comentarioRepository;
        _estadoRepository = estadoRepository;
    }

    public async Task<IEnumerable<SolicitudDto>> GetAllAsync()
    {
        var solicitudes = await _solicitudRepository.GetAllAsync();
        return MapToDto(solicitudes);
    }

    public async Task<IEnumerable<SolicitudDto>> GetByFiltrosAsync(
        int? idEstado, int? idArea, int? idPrioridad,
        DateTime? fechaDesde, DateTime? fechaHasta, string? busqueda)
    {
        var solicitudes = await _solicitudRepository.GetByFiltrosAsync(
            idEstado, idArea, idPrioridad, fechaDesde, fechaHasta, busqueda);
        return MapToDto(solicitudes);
    }

    public async Task<IEnumerable<SolicitudDto>> GetBySolicitanteAsync(int idSolicitante)
    {
        var solicitudes = await _solicitudRepository.GetBySolicitanteAsync(idSolicitante);
        return MapToDto(solicitudes);
    }

    public async Task<IEnumerable<SolicitudDto>> GetByGestorAsync(int idGestor)
    {
        var solicitudes = await _solicitudRepository.GetByGestorAsync(idGestor);
        return MapToDto(solicitudes);
    }

    public async Task<IEnumerable<SolicitudDto>> GetByAreaAsync(int idArea)
    {
        var solicitudes = await _solicitudRepository.GetByAreaAsync(idArea);
        return MapToDto(solicitudes);
    }

    public async Task<SolicitudDetalleDto?> GetDetalleAsync(int id)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id);
        if (solicitud == null)
            return null;

        var trazabilidad = await _trazabilidadRepository.GetBySolicitudAsync(id);
        var comentarios = await _comentarioRepository.GetBySolicitudAsync(id);

        return new SolicitudDetalleDto
        {
            Solicitud = MapToDto(new[] { solicitud }).First(),
            Trazabilidad = trazabilidad.Select(t => new TrazabilidadDto
            {
                IdTrazabilidad = t.IdTrazabilidad,
                Accion = t.Accion,
                Descripcion = t.Descripcion,
                FechaEvento = t.FechaEvento,
                NombreUsuario = t.UsuarioActor?.NombreCompleto
            }).ToList(),
            Comentarios = comentarios.Select(c => new ComentarioDto
            {
                IdComentario = c.IdComentario,
                Texto = c.Texto,
                FechaComentario = c.FechaComentario,
                IdUsuario = c.IdUsuario,
                NombreUsuario = c.Usuario.NombreCompleto
            }).ToList()
        };
    }

    public async Task<SolicitudDto> CreateAsync(CreateSolicitudRequest request, int idSolicitante)
    {
        // Obtener el estado "Nueva" (asumimos que tiene ID 1)
        var estadoNueva = await _estadoRepository.GetByIdAsync(1);
        if (estadoNueva == null)
            throw new InvalidOperationException("Estado 'Nueva' no encontrado");

        var solicitud = new Solicitud
        {
            Asunto = request.Asunto,
            Descripcion = request.Descripcion,
            ArchivoUrl = request.ArchivoUrl,
            IdSolicitante = idSolicitante,
            IdArea = request.IdArea,
            IdTipoSolicitud = request.IdTipoSolicitud,
            IdPrioridad = request.IdPrioridad,
            IdEstado = 1, // Nueva
            FechaCreacion = DateTime.UtcNow
        };

        var created = await _solicitudRepository.AddAsync(solicitud);

        // Registrar trazabilidad
        await _trazabilidadRepository.AddAsync(new TrazabilidadSolicitud
        {
            IdSolicitud = created.IdSolicitud,
            IdUsuarioActor = idSolicitante,
            Accion = "CREACION",
            Descripcion = $"Solicitud creada: {created.Asunto}",
            FechaEvento = DateTime.UtcNow
        });

        return MapToDto(new[] { created }).First();
    }

    public async Task<SolicitudDto> UpdateAsync(int id, UpdateSolicitudRequest request, int idUsuario)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id);
        if (solicitud == null)
            throw new InvalidOperationException("Solicitud no encontrada");

        // Validar que solo se puede editar si esta en estado "Nueva"
        if (solicitud.IdEstado != 1)
            throw new InvalidOperationException("Solo se puede editar una solicitud en estado 'Nueva'");

        solicitud.Asunto = request.Asunto;
        solicitud.Descripcion = request.Descripcion;
        solicitud.IdPrioridad = request.IdPrioridad;

        await _solicitudRepository.UpdateAsync(solicitud);

        // Registrar trazabilidad
        await _trazabilidadRepository.AddAsync(new TrazabilidadSolicitud
        {
            IdSolicitud = solicitud.IdSolicitud,
            IdUsuarioActor = idUsuario,
            Accion = "EDICION",
            Descripcion = "Solicitud editada",
            FechaEvento = DateTime.UtcNow
        });

        return MapToDto(new[] { solicitud }).First();
    }

    public async Task CambiarEstadoAsync(int id, CambiarEstadoRequest request, int idUsuario)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id);
        if (solicitud == null)
            throw new InvalidOperationException("Solicitud no encontrada");

        var estadoAnterior = solicitud.Estado.Nombre;
        solicitud.IdEstado = request.IdEstado;

        await _solicitudRepository.UpdateAsync(solicitud);

        var estadoNuevo = await _estadoRepository.GetByIdAsync(request.IdEstado);

        // Registrar trazabilidad
        await _trazabilidadRepository.AddAsync(new TrazabilidadSolicitud
        {
            IdSolicitud = solicitud.IdSolicitud,
            IdUsuarioActor = idUsuario,
            Accion = "CAMBIO_ESTADO",
            Descripcion = $"Estado cambiado de '{estadoAnterior}' a '{estadoNuevo?.Nombre}'",
            FechaEvento = DateTime.UtcNow
        });

        // Agregar comentario si se proporcion�
        if (!string.IsNullOrWhiteSpace(request.Comentario))
        {
            await _comentarioRepository.AddAsync(new Comentario
            {
                IdSolicitud = solicitud.IdSolicitud,
                IdUsuario = idUsuario,
                Texto = request.Comentario,
                FechaComentario = DateTime.UtcNow
            });
        }
    }

    public async Task AsignarGestorAsync(int id, AsignarGestorRequest request, int idAsignadoPor)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id);
        if (solicitud == null)
            throw new InvalidOperationException("Solicitud no encontrada");

        solicitud.IdGestorAsignado = request.IdGestor;
        solicitud.IdAsignadoPor = idAsignadoPor;
        solicitud.FechaAsignacion = DateTime.UtcNow;

        await _solicitudRepository.UpdateAsync(solicitud);

        // Registrar trazabilidad
        var gestor = await _solicitudRepository.GetByIdAsync(id);
        await _trazabilidadRepository.AddAsync(new TrazabilidadSolicitud
        {
            IdSolicitud = solicitud.IdSolicitud,
            IdUsuarioActor = idAsignadoPor,
            Accion = "ASIGNACION",
            Descripcion = $"Asignado a {gestor?.GestorAsignado?.NombreCompleto}",
            FechaEvento = DateTime.UtcNow
        });
    }

    public async Task TomarSolicitudAsync(int id, int idGestor)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id);
        if (solicitud == null)
            throw new InvalidOperationException("Solicitud no encontrada");

        solicitud.IdGestorAsignado = idGestor;
        solicitud.IdAsignadoPor = idGestor; // El mismo gestor se asigna
        solicitud.FechaAsignacion = DateTime.UtcNow;

        await _solicitudRepository.UpdateAsync(solicitud);

        // Registrar trazabilidad
        await _trazabilidadRepository.AddAsync(new TrazabilidadSolicitud
        {
            IdSolicitud = solicitud.IdSolicitud,
            IdUsuarioActor = idGestor,
            Accion = "TOMAR_SOLICITUD",
            Descripcion = "Gestor toma la solicitud",
            FechaEvento = DateTime.UtcNow
        });
    }

    public async Task DeleteAsync(int id)
    {
        await _solicitudRepository.DeleteAsync(id);
    }

    private static IEnumerable<SolicitudDto> MapToDto(IEnumerable<Solicitud> solicitudes)
    {
        return solicitudes.Select(s => new SolicitudDto
        {
            IdSolicitud = s.IdSolicitud,
            Codigo = s.Codigo,
            Asunto = s.Asunto,
            Descripcion = s.Descripcion,
            ArchivoUrl = s.ArchivoUrl,
            FechaCreacion = s.FechaCreacion,
            IdSolicitante = s.IdSolicitante,
            NombreSolicitante = s.Solicitante.NombreCompleto,
            IdArea = s.IdArea,
            NombreArea = s.Area.Nombre,
            IdTipoSolicitud = s.IdTipoSolicitud,
            NombreTipoSolicitud = s.TipoSolicitud.Nombre,
            IdPrioridad = s.IdPrioridad,
            NombrePrioridad = s.Prioridad.Nombre,
            IdEstado = s.IdEstado,
            NombreEstado = s.Estado.Nombre,
            IdGestorAsignado = s.IdGestorAsignado,
            NombreGestorAsignado = s.GestorAsignado?.NombreCompleto,
            FechaAsignacion = s.FechaAsignacion
        });
    }
}
