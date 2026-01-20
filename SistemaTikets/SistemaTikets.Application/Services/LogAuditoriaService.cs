using SistemaTikets.Application.DTOs.Auditoria;
using SistemaTikets.Domain.Interfaces;

namespace SistemaTikets.Application.Services;

public interface ILogAuditoriaService
{
    Task<IEnumerable<LogAuditoriaDto>> GetAllLogsAsync(int pageNumber = 1, int pageSize = 50);
    Task<IEnumerable<LogAuditoriaDto>> GetLogsByUsuarioAsync(int idUsuario);
    Task<IEnumerable<LogAuditoriaDto>> GetLogsByEntidadAsync(string entidad, int? idEntidad = null);
    Task<IEnumerable<LogAuditoriaDto>> GetLogsByFechasAsync(DateTime fechaInicio, DateTime fechaFin, int pageNumber = 1, int pageSize = 50);
    Task<IEnumerable<LogAuditoriaDto>> GetLogsByTipoAccionAsync(string tipoAccion);
    Task<IEnumerable<LogAuditoriaDto>> GetLogsConFiltrosAsync(FiltroLogsRequest filtro);
    Task<int> GetTotalCountAsync();
    Task<int> GetCountConFiltrosAsync(FiltroLogsRequest filtro);
}

public class LogAuditoriaService : ILogAuditoriaService
{
    private readonly ILogAuditoriaRepository _logRepository;

    public LogAuditoriaService(ILogAuditoriaRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<IEnumerable<LogAuditoriaDto>> GetAllLogsAsync(int pageNumber = 1, int pageSize = 50)
    {
        var logs = await _logRepository.GetAllAsync(pageNumber, pageSize);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<LogAuditoriaDto>> GetLogsByUsuarioAsync(int idUsuario)
    {
        var logs = await _logRepository.GetByUsuarioAsync(idUsuario);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<LogAuditoriaDto>> GetLogsByEntidadAsync(string entidad, int? idEntidad = null)
    {
        var logs = await _logRepository.GetByEntidadAsync(entidad, idEntidad);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<LogAuditoriaDto>> GetLogsByFechasAsync(DateTime fechaInicio, DateTime fechaFin, int pageNumber = 1, int pageSize = 50)
    {
        var logs = await _logRepository.GetByFechasAsync(fechaInicio, fechaFin, pageNumber, pageSize);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<LogAuditoriaDto>> GetLogsByTipoAccionAsync(string tipoAccion)
    {
        var logs = await _logRepository.GetByTipoAccionAsync(tipoAccion);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<LogAuditoriaDto>> GetLogsConFiltrosAsync(FiltroLogsRequest filtro)
    {
        var logs = await _logRepository.GetByFiltrosAsync(
            filtro.IdUsuario,
            filtro.TipoAccion,
            filtro.EntidadAfectada,
            filtro.IdEntidad,
            filtro.FechaInicio,
            filtro.FechaFin,
            filtro.Resultado,
            filtro.PageNumber,
            filtro.PageSize
        );
        return logs.Select(MapToDto);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _logRepository.GetTotalCountAsync();
    }

    public async Task<int> GetCountConFiltrosAsync(FiltroLogsRequest filtro)
    {
        return await _logRepository.GetCountByFiltrosAsync(
            filtro.IdUsuario,
            filtro.TipoAccion,
            filtro.EntidadAfectada,
            filtro.IdEntidad,
            filtro.FechaInicio,
            filtro.FechaFin,
            filtro.Resultado
        );
    }

    private LogAuditoriaDto MapToDto(Domain.Entities.LogAuditoria log)
    {
        return new LogAuditoriaDto
        {
            IdLog = log.IdLog,
            IdUsuario = log.IdUsuario,
            NombreUsuario = log.Usuario?.NombreCompleto,
            TipoAccion = log.TipoAccion,
            EntidadAfectada = log.EntidadAfectada,
            IdEntidad = log.IdEntidad,
            ValoresAnteriores = log.ValoresAnteriores,
            ValoresNuevos = log.ValoresNuevos,
            IpOrigen = log.IpOrigen,
            Descripcion = log.Descripcion,
            Resultado = log.Resultado,
            FechaEvento = log.FechaEvento
        };
    }
}
