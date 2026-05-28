using Eventix.Application.DTOs.AuditLog;
using Eventix.Application.DTOs.Common;
using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        Task<PagedResult<AuditLog>> GetPagedAsync(
            AuditLogQueryDTO query,
            CancellationToken cancellationToken = default);

        Task<AuditLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}