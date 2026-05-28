using Eventix.Application.DTOs.AuditLog;
using Eventix.Application.DTOs.Common;

namespace Eventix.Application.Interfaces.Services;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogDTO>> GetPagedAsync(
        AuditLogQueryDTO query,
        CancellationToken cancellationToken = default);

    Task<AuditLogDTO?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        CreateAuditLogDTO dto,
        CancellationToken cancellationToken = default);
}