using SistemaTikets.Application.DTOs.Catalogos;
using SistemaTikets.Domain.Entities;
using SistemaTikets.Domain.Interfaces;


namespace SistemaTikets.Application.Services;

public interface ICatalogoService
{
    // Areas
    Task<IEnumerable<AreaDto>> GetAllAreasAsync();
    Task<AreaDto?> GetAreaByIdAsync(int id);
    Task<AreaDto> CreateAreaAsync(CreateAreaRequest request);
    Task<AreaDto> UpdateAreaAsync(int id, CreateAreaRequest request);
    Task DeleteAreaAsync(int id);

    // Tipos de Solicitud
    Task<IEnumerable<TipoSolicitudDto>> GetAllTiposSolicitudAsync();
    Task<IEnumerable<TipoSolicitudDto>> GetTiposSolicitudByAreaAsync(int idArea);
    Task<TipoSolicitudDto?> GetTipoSolicitudByIdAsync(int id);
    Task<TipoSolicitudDto> CreateTipoSolicitudAsync(CreateTipoSolicitudRequest request);
    Task<TipoSolicitudDto> UpdateTipoSolicitudAsync(int id, CreateTipoSolicitudRequest request);
    Task DeleteTipoSolicitudAsync(int id);

    // Prioridades
    Task<IEnumerable<PrioridadDto>> GetAllPrioridadesAsync();
    Task<PrioridadDto?> GetPrioridadByIdAsync(int id);
    Task<PrioridadDto> CreatePrioridadAsync(CreatePrioridadRequest request);
    Task<PrioridadDto> UpdatePrioridadAsync(int id, CreatePrioridadRequest request);
    Task DeletePrioridadAsync(int id);

    // Estados
    Task<IEnumerable<EstadoDto>> GetAllEstadosAsync();
    Task<EstadoDto?> GetEstadoByIdAsync(int id);
    Task<EstadoDto> CreateEstadoAsync(CreateEstadoRequest request);
    Task<EstadoDto> UpdateEstadoAsync(int id, CreateEstadoRequest request);
    Task DeleteEstadoAsync(int id);

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

    public CatalogoService(
        IAreaRepository areaRepository,
        ITipoSolicitudRepository tipoSolicitudRepository,
        IPrioridadRepository prioridadRepository,
        IEstadoRepository estadoRepository,
        IRolRepository rolRepository)
    {
        _areaRepository = areaRepository;
        _tipoSolicitudRepository = tipoSolicitudRepository;
        _prioridadRepository = prioridadRepository;
        _estadoRepository = estadoRepository;
        _rolRepository = rolRepository;
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

    public async Task<AreaDto> CreateAreaAsync(CreateAreaRequest request)
    {
        var area = new Area { Nombre = request.Nombre };
        var created = await _areaRepository.AddAsync(area);
        return new AreaDto { IdArea = created.IdArea, Nombre = created.Nombre };
    }

    public async Task<AreaDto> UpdateAreaAsync(int id, CreateAreaRequest request)
    {
        var area = await _areaRepository.GetByIdAsync(id);
        if (area == null)
            throw new InvalidOperationException("�rea no encontrada");

        area.Nombre = request.Nombre;
        await _areaRepository.UpdateAsync(area);
        return new AreaDto { IdArea = area.IdArea, Nombre = area.Nombre };
    }

    public async Task DeleteAreaAsync(int id)
    {
        await _areaRepository.DeleteAsync(id);
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

    public async Task<TipoSolicitudDto> CreateTipoSolicitudAsync(CreateTipoSolicitudRequest request)
    {
        var tipo = new TipoSolicitud { Nombre = request.Nombre, IdArea = request.IdArea };
        var created = await _tipoSolicitudRepository.AddAsync(tipo);
        var area = await _areaRepository.GetByIdAsync(created.IdArea);
        return new TipoSolicitudDto
        {
            IdTipoSolicitud = created.IdTipoSolicitud,
            Nombre = created.Nombre,
            IdArea = created.IdArea,
            NombreArea = area?.Nombre ?? string.Empty
        };
    }

    public async Task<TipoSolicitudDto> UpdateTipoSolicitudAsync(int id, CreateTipoSolicitudRequest request)
    {
        var tipo = await _tipoSolicitudRepository.GetByIdAsync(id);
        if (tipo == null)
            throw new InvalidOperationException("Tipo de solicitud no encontrado");

        tipo.Nombre = request.Nombre;
        tipo.IdArea = request.IdArea;
        await _tipoSolicitudRepository.UpdateAsync(tipo);

        var area = await _areaRepository.GetByIdAsync(tipo.IdArea);
        return new TipoSolicitudDto
        {
            IdTipoSolicitud = tipo.IdTipoSolicitud,
            Nombre = tipo.Nombre,
            IdArea = tipo.IdArea,
            NombreArea = area?.Nombre ?? string.Empty
        };
    }

    public async Task DeleteTipoSolicitudAsync(int id)
    {
        await _tipoSolicitudRepository.DeleteAsync(id);
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

    public async Task<PrioridadDto> CreatePrioridadAsync(CreatePrioridadRequest request)
    {
        var prioridad = new Prioridad { Nombre = request.Nombre };
        var created = await _prioridadRepository.AddAsync(prioridad);
        return new PrioridadDto { IdPrioridad = created.IdPrioridad, Nombre = created.Nombre };
    }

    public async Task<PrioridadDto> UpdatePrioridadAsync(int id, CreatePrioridadRequest request)
    {
        var prioridad = await _prioridadRepository.GetByIdAsync(id);
        if (prioridad == null)
            throw new InvalidOperationException("Prioridad no encontrada");

        prioridad.Nombre = request.Nombre;
        await _prioridadRepository.UpdateAsync(prioridad);
        return new PrioridadDto { IdPrioridad = prioridad.IdPrioridad, Nombre = prioridad.Nombre };
    }

    public async Task DeletePrioridadAsync(int id)
    {
        await _prioridadRepository.DeleteAsync(id);
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

    public async Task<EstadoDto> CreateEstadoAsync(CreateEstadoRequest request)
    {
        var estado = new Estado { Nombre = request.Nombre };
        var created = await _estadoRepository.AddAsync(estado);
        return new EstadoDto { IdEstado = created.IdEstado, Nombre = created.Nombre };
    }

    public async Task<EstadoDto> UpdateEstadoAsync(int id, CreateEstadoRequest request)
    {
        var estado = await _estadoRepository.GetByIdAsync(id);
        if (estado == null)
            throw new InvalidOperationException("Estado no encontrado");

        estado.Nombre = request.Nombre;
        await _estadoRepository.UpdateAsync(estado);
        return new EstadoDto { IdEstado = estado.IdEstado, Nombre = estado.Nombre };
    }

    public async Task DeleteEstadoAsync(int id)
    {
        await _estadoRepository.DeleteAsync(id);
    }

    // ROLES
    public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
    {
        var roles = await _rolRepository.GetAllAsync();
        return roles.Select(r => new RolDto { IdRol = r.IdRol, Nombre = r.Nombre });
    }
}
