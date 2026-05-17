using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly TenantDbContext _context;

        public AuditLogRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync(); 
        }

        public async Task<AuditLog?> GetByIdAsync(Guid id)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<AuditLog>> GetByUserIdAsync(Guid userId)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByEntityAsync(string entityName, Guid entityId)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }

        public void Update(AuditLog auditLog)
        {
            _context.AuditLogs.Update(auditLog);
        }

        public void Delete(AuditLog auditLog)
        {
            _context.AuditLogs.Remove(auditLog);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}