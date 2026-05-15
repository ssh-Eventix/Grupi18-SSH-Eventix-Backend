using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        Task<List<AuditLog>> GetAllAsync();

        Task<AuditLog?> GetByIdAsync(Guid id);

        Task<List<AuditLog>> GetByUserIdAsync(Guid userId);

        Task<List<AuditLog>> GetByEntityAsync(string entityName, Guid entityId);

        Task AddAsync(AuditLog auditLog);

        void Update(AuditLog auditLog);

        void Delete(AuditLog auditLog);

        Task SaveChangesAsync();
    }
}