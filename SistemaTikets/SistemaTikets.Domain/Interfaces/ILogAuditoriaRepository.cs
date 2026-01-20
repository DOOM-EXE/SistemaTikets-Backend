using SistemaTikets.Domain.Entities;

namespace SistemaTikets.Domain.Interfaces;

public interface ILogAuditoriaRepository
{
    Task<LogAuditoria> CreateAsync(LogAuditoria log);
    Task<IEnumerable<LogAuditoria>> GetAllAsync(int pageNumber = 1, int pageSize = 50);
    Task<IEnumerable<LogAuditoria>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<LogAuditoria>> GetByEntidadAsync(string entidad, int? idEntidad = null);
    Task<IEnumerable<LogAuditoria>> GetByFechasAsync(DateTime fechaInicio, DateTime fechaFin, int pageNumber = 1, int pageSize = 50);
    Task<IEnumerable<LogAuditoria>> GetByTipoAccionAsync(string tipoAccion);
    Task<LogAuditoria?> GetByIdAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<LogAuditoria>> GetByFiltrosAsync(
        int? idUsuario,
        string? tipoAccion,
        string? entidadAfectada,
        int? idEntidad,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? resultado,
        int pageNumber = 1,
        int pageSize = 50
    );
    Task<int> GetCountByFiltrosAsync(
        int? idUsuario,
        string? tipoAccion,
        string? entidadAfectada,
        int? idEntidad,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? resultado
    );
}
