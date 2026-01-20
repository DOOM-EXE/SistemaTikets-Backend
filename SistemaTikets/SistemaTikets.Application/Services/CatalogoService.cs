using SistemaTikets.Application.DTOs.Catalogos;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;


namespace SistemaTikets.Application.Services;

public interface ICatalogoService
{
    // Areas
    Task<IEnumerable<AreaDto>> GetAllAreasAsync();
    Task<AreaDto?> GetAreaByIdAsync(int id);
    Task<AreaDto> CreateAreaAsync(CreateAreaRequest request, int idUsuario, string? ipAddress = null);
    Task<AreaDto> UpdateAreaAsync(int id, CreateAreaRequest request, int idUsuario, string? ipAddress = null);
    Task DeleteAreaAsync(int id, int idUsuario, string? ipAddress = null);

    // Tipos de Solicitud
    Task<IEnumerable<TipoSolicitudDto>> GetAllTiposSolicitudAsync();
    Task<IEnumerable<TipoSolicitudDto>> GetTiposSolicitudByAreaAsync(int idArea);
    Task<TipoSolicitudDto?> GetTipoSolicitudByIdAsync(int id);
    Task<TipoSolicitudDto> CreateTipoSolicitudAsync(CreateTipoSolicitudRequest request, int idUsuario, string? ipAddress = null);
    Task<TipoSolicitudDto> UpdateTipoSolicitudAsync(int id, CreateTipoSolicitudRequest request, int idUsuario, string? ipAddress = null);
    Task DeleteTipoSolicitudAsync(int id, int idUsuario, string? ipAddress = null);

    // Prioridades
    Task<IEnumerable<PrioridadDto>> GetAllPrioridadesAsync();
    Task<PrioridadDto?> GetPrioridadByIdAsync(int id);
    Task<PrioridadDto> CreatePrioridadAsync(CreatePrioridadRequest request, int idUsuario, string? ipAddress = null);
    Task<PrioridadDto> UpdatePrioridadAsync(int id, CreatePrioridadRequest request, int idUsuario, string? ipAddress = null);
    Task DeletePrioridadAsync(int id, int idUsuario, string? ipAddress = null);

    // Estados
    Task<IEnumerable<EstadoDto>> GetAllEstadosAsync();
    Task<EstadoDto?> GetEstadoByIdAsync(int id);
    Task<EstadoDto> CreateEstadoAsync(CreateEstadoRequest request, int idUsuario, string? ipAddress = null);
    Task<EstadoDto> UpdateEstadoAsync(int id, CreateEstadoRequest request, int idUsuario, string? ipAddress = null);
    Task DeleteEstadoAsync(int id, int idUsuario, string? ipAddress = null);

    // Roles
    Task<IEnumerable<RolDto>> GetAllRolesAsync();
}

public class CatalogoService : ICatalogoService
{
    private readonly IAreaRepository _areaRepository;
    private readonly ITipoSolicitudRepository _tipoSolicitudRepository;
    private readonly IPrioridadRepository _prioridadRepository;
    private readonly IEstadoRepository _estadoRepository;
    private readonly IRolRepository _rolRepository;
    private readonly ISolicitudRepository _solicitudRepository;
    private readonly IAuditoriaService? _auditoriaService;

    public CatalogoService(
        IAreaRepository areaRepository,
        ITipoSolicitudRepository tipoSolicitudRepository,
        IPrioridadRepository prioridadRepository,
        IEstadoRepository estadoRepository,
        IRolRepository rolRepository,
        ISolicitudRepository solicitudRepository,
        IAuditoriaService? auditoriaService = null)
    {
        _areaRepository = areaRepository;
        _tipoSolicitudRepository = tipoSolicitudRepository;
        _prioridadRepository = prioridadRepository;
        _estadoRepository = estadoRepository;
        _rolRepository = rolRepository;
        _solicitudRepository = solicitudRepository;
        _auditoriaService = auditoriaService;
    }

    // AREAS
    public async Task<IEnumerable<AreaDto>> GetAllAreasAsync()
    {
        var areas = await _areaRepository.GetAllAsync();
        return areas.Select(a => new AreaDto { IdArea = a.IdArea, Nombre = a.Nombre });
    }

    public async Task<AreaDto?> GetAreaByIdAsync(int id)
    {
        var area = await _areaRepository.GetByIdAsync(id);
        return area == null ? null : new AreaDto { IdArea = area.IdArea, Nombre = area.Nombre };
    }

    public async Task<AreaDto> CreateAreaAsync(CreateAreaRequest request, int idUsuario, string? ipAddress = null)
    {
        var area = new Area { Nombre = request.Nombre };
        var created = await _areaRepository.AddAsync(area);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarCreacionCatalogo(idUsuario, "Area", created.IdArea, created.Nombre, ipAddress);
        }

        return new AreaDto { IdArea = created.IdArea, Nombre = created.Nombre };
    }

    public async Task<AreaDto> UpdateAreaAsync(int id, CreateAreaRequest request, int idUsuario, string? ipAddress = null)
    {
        var area = await _areaRepository.GetByIdAsync(id);
        if (area == null)
            throw new InvalidOperationException("Área no encontrada");

        var nombreAnterior = area.Nombre;
        area.Nombre = request.Nombre;
        await _areaRepository.UpdateAsync(area);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEdicionCatalogo(idUsuario, "Area", id, nombreAnterior, area.Nombre, ipAddress);
        }

        return new AreaDto { IdArea = area.IdArea, Nombre = area.Nombre };
    }

    public async Task DeleteAreaAsync(int id, int idUsuario, string? ipAddress = null)
    {
        var area = await _areaRepository.GetByIdAsync(id);
        if (area == null)
            throw new InvalidOperationException("Área no encontrada");

        // Verificar si hay solicitudes usando esta área
        var solicitudes = await _solicitudRepository.GetByAreaAsync(id);
        if (solicitudes.Any())
            throw new InvalidOperationException("No se puede eliminar el área porque tiene solicitudes asociadas");

        var nombreArea = area.Nombre;
        await _areaRepository.DeleteAsync(id);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEliminacionCatalogo(idUsuario, "Area", id, nombreArea, ipAddress);
        }
    }

    // TIPOS DE SOLICITUD
    public async Task<IEnumerable<TipoSolicitudDto>> GetAllTiposSolicitudAsync()
    {
        var tipos = await _tipoSolicitudRepository.GetAllAsync();
        return tipos.Select(t => new TipoSolicitudDto
        {
            IdTipoSolicitud = t.IdTipoSolicitud,
            Nombre = t.Nombre,
            IdArea = t.IdArea,
            NombreArea = t.Area.Nombre
        });
    }

    public async Task<IEnumerable<TipoSolicitudDto>> GetTiposSolicitudByAreaAsync(int idArea)
    {
        var tipos = await _tipoSolicitudRepository.GetByAreaAsync(idArea);
        var area = await _areaRepository.GetByIdAsync(idArea);
        return tipos.Select(t => new TipoSolicitudDto
        {
            IdTipoSolicitud = t.IdTipoSolicitud,
            Nombre = t.Nombre,
            IdArea = t.IdArea,
            NombreArea = area?.Nombre ?? string.Empty
        });
    }

    public async Task<TipoSolicitudDto?> GetTipoSolicitudByIdAsync(int id)
    {
        var tipo = await _tipoSolicitudRepository.GetByIdAsync(id);
        return tipo == null ? null : new TipoSolicitudDto
        {
            IdTipoSolicitud = tipo.IdTipoSolicitud,
            Nombre = tipo.Nombre,
            IdArea = tipo.IdArea,
            NombreArea = tipo.Area.Nombre
        };
    }

    public async Task<TipoSolicitudDto> CreateTipoSolicitudAsync(CreateTipoSolicitudRequest request, int idUsuario, string? ipAddress = null)
    {
        var tipo = new TipoSolicitud { Nombre = request.Nombre, IdArea = request.IdArea };
        var created = await _tipoSolicitudRepository.AddAsync(tipo);
        var area = await _areaRepository.GetByIdAsync(created.IdArea);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarCreacionCatalogo(idUsuario, "TipoSolicitud", created.IdTipoSolicitud, created.Nombre, ipAddress);
        }

        return new TipoSolicitudDto
        {
            IdTipoSolicitud = created.IdTipoSolicitud,
            Nombre = created.Nombre,
            IdArea = created.IdArea,
            NombreArea = area?.Nombre ?? string.Empty
        };
    }

    public async Task<TipoSolicitudDto> UpdateTipoSolicitudAsync(int id, CreateTipoSolicitudRequest request, int idUsuario, string? ipAddress = null)
    {
        var tipo = await _tipoSolicitudRepository.GetByIdAsync(id);
        if (tipo == null)
            throw new InvalidOperationException("Tipo de solicitud no encontrado");

        var nombreAnterior = tipo.Nombre;
        tipo.Nombre = request.Nombre;
        tipo.IdArea = request.IdArea;
        await _tipoSolicitudRepository.UpdateAsync(tipo);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEdicionCatalogo(idUsuario, "TipoSolicitud", id, nombreAnterior, tipo.Nombre, ipAddress);
        }

        var area = await _areaRepository.GetByIdAsync(tipo.IdArea);
        return new TipoSolicitudDto
        {
            IdTipoSolicitud = tipo.IdTipoSolicitud,
            Nombre = tipo.Nombre,
            IdArea = tipo.IdArea,
            NombreArea = area?.Nombre ?? string.Empty
        };
    }

    public async Task DeleteTipoSolicitudAsync(int id, int idUsuario, string? ipAddress = null)
    {
        var tipoSolicitud = await _tipoSolicitudRepository.GetByIdAsync(id);
        if (tipoSolicitud == null)
            throw new InvalidOperationException("Tipo de solicitud no encontrado");

        // Verificar si hay solicitudes usando este tipo
        var solicitudes = await _solicitudRepository.GetAllAsync();
        if (solicitudes.Any(s => s.IdTipoSolicitud == id))
            throw new InvalidOperationException("No se puede eliminar el tipo de solicitud porque tiene solicitudes asociadas");

        var nombreTipo = tipoSolicitud.Nombre;
        await _tipoSolicitudRepository.DeleteAsync(id);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEliminacionCatalogo(idUsuario, "TipoSolicitud", id, nombreTipo, ipAddress);
        }
    }

    // PRIORIDADES
    public async Task<IEnumerable<PrioridadDto>> GetAllPrioridadesAsync()
    {
        var prioridades = await _prioridadRepository.GetAllAsync();
        return prioridades.Select(p => new PrioridadDto { IdPrioridad = p.IdPrioridad, Nombre = p.Nombre });
    }

    public async Task<PrioridadDto?> GetPrioridadByIdAsync(int id)
    {
        var prioridad = await _prioridadRepository.GetByIdAsync(id);
        return prioridad == null ? null : new PrioridadDto { IdPrioridad = prioridad.IdPrioridad, Nombre = prioridad.Nombre };
    }

    public async Task<PrioridadDto> CreatePrioridadAsync(CreatePrioridadRequest request, int idUsuario, string? ipAddress = null)
    {
        var prioridad = new Prioridad { Nombre = request.Nombre };
        var created = await _prioridadRepository.AddAsync(prioridad);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarCreacionCatalogo(idUsuario, "Prioridad", created.IdPrioridad, created.Nombre, ipAddress);
        }

        return new PrioridadDto { IdPrioridad = created.IdPrioridad, Nombre = created.Nombre };
    }

    public async Task<PrioridadDto> UpdatePrioridadAsync(int id, CreatePrioridadRequest request, int idUsuario, string? ipAddress = null)
    {
        var prioridad = await _prioridadRepository.GetByIdAsync(id);
        if (prioridad == null)
            throw new InvalidOperationException("Prioridad no encontrada");

        var nombreAnterior = prioridad.Nombre;
        prioridad.Nombre = request.Nombre;
        await _prioridadRepository.UpdateAsync(prioridad);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEdicionCatalogo(idUsuario, "Prioridad", id, nombreAnterior, prioridad.Nombre, ipAddress);
        }

        return new PrioridadDto { IdPrioridad = prioridad.IdPrioridad, Nombre = prioridad.Nombre };
    }

    public async Task DeletePrioridadAsync(int id, int idUsuario, string? ipAddress = null)
    {
        var prioridad = await _prioridadRepository.GetByIdAsync(id);
        if (prioridad == null)
            throw new InvalidOperationException("Prioridad no encontrada");

        // Verificar si hay solicitudes usando esta prioridad
        var solicitudes = await _solicitudRepository.GetAllAsync();
        if (solicitudes.Any(s => s.IdPrioridad == id))
            throw new InvalidOperationException("No se puede eliminar la prioridad porque tiene solicitudes asociadas");

        var nombrePrioridad = prioridad.Nombre;
        await _prioridadRepository.DeleteAsync(id);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEliminacionCatalogo(idUsuario, "Prioridad", id, nombrePrioridad, ipAddress);
        }
    }

    // ESTADOS
    public async Task<IEnumerable<EstadoDto>> GetAllEstadosAsync()
    {
        var estados = await _estadoRepository.GetAllAsync();
        return estados.Select(e => new EstadoDto { IdEstado = e.IdEstado, Nombre = e.Nombre });
    }

    public async Task<EstadoDto?> GetEstadoByIdAsync(int id)
    {
        var estado = await _estadoRepository.GetByIdAsync(id);
        return estado == null ? null : new EstadoDto { IdEstado = estado.IdEstado, Nombre = estado.Nombre };
    }

    public async Task<EstadoDto> CreateEstadoAsync(CreateEstadoRequest request, int idUsuario, string? ipAddress = null)
    {
        var estado = new Estado { Nombre = request.Nombre };
        var created = await _estadoRepository.AddAsync(estado);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarCreacionCatalogo(idUsuario, "Estado", created.IdEstado, created.Nombre, ipAddress);
        }

        return new EstadoDto { IdEstado = created.IdEstado, Nombre = created.Nombre };
    }

    public async Task<EstadoDto> UpdateEstadoAsync(int id, CreateEstadoRequest request, int idUsuario, string? ipAddress = null)
    {
        var estado = await _estadoRepository.GetByIdAsync(id);
        if (estado == null)
            throw new InvalidOperationException("Estado no encontrado");

        var nombreAnterior = estado.Nombre;
        estado.Nombre = request.Nombre;
        await _estadoRepository.UpdateAsync(estado);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEdicionCatalogo(idUsuario, "Estado", id, nombreAnterior, estado.Nombre, ipAddress);
        }

        return new EstadoDto { IdEstado = estado.IdEstado, Nombre = estado.Nombre };
    }

    public async Task DeleteEstadoAsync(int id, int idUsuario, string? ipAddress = null)
    {
        var estado = await _estadoRepository.GetByIdAsync(id);
        if (estado == null)
            throw new InvalidOperationException("Estado no encontrado");

        // Verificar si hay solicitudes usando este estado
        var solicitudes = await _solicitudRepository.GetAllAsync();
        if (solicitudes.Any(s => s.IdEstado == id))
            throw new InvalidOperationException("No se puede eliminar el estado porque tiene solicitudes asociadas");

        var nombreEstado = estado.Nombre;
        await _estadoRepository.DeleteAsync(id);

        if (_auditoriaService != null)
        {
            await _auditoriaService.RegistrarEliminacionCatalogo(idUsuario, "Estado", id, nombreEstado, ipAddress);
        }
    }

    // ROLES
    public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
    {
        var roles = await _rolRepository.GetAllAsync();
        return roles.Select(r => new RolDto { IdRol = r.IdRol, Nombre = r.Nombre });
    }
}
