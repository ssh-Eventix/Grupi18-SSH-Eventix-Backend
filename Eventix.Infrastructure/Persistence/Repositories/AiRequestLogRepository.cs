using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories
{
    public class AIRequestLogRepository : IAIRequestLogRepository
    {
        private readonly TenantDbContext _context;

        public AIRequestLogRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<List<AIRequestLog>> GetAllAsync()
        {
            return await _context.AIRequestLogs
                .AsNoTracking()
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<AIRequestLog?> GetByIdAsync(Guid id)
        {
            return await _context.AIRequestLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<AIRequestLog>> GetByUserIdAsync(Guid userId)
        {
            return await _context.AIRequestLogs
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<AIRequestLog>> GetByStatusAsync(AIRequestStatus status)
        {
            return await _context.AIRequestLogs
                .AsNoTracking()
                .Where(a => a.Status == status)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<AIRequestLog>> GetByRequestTypeAsync(AIRequestType requestType)
        {
            return await _context.AIRequestLogs
                .AsNoTracking()
                .Where(a => a.RequestType == requestType)
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task AddAsync(AIRequestLog aiRequestLog)
        {
            await _context.AIRequestLogs.AddAsync(aiRequestLog);
        }

        public void Update(AIRequestLog aiRequestLog)
        {
            _context.AIRequestLogs.Update(aiRequestLog);
        }

        public void Delete(AIRequestLog aiRequestLog)
        {
            _context.AIRequestLogs.Remove(aiRequestLog);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}