using Eventix.Application.DTOs.AuditLog;
using Eventix.Application.DTOs.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly TenantDbContext _context;

    public AuditLogRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AuditLog>> GetPagedAsync(
        AuditLogQueryDTO query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        pageSize = Math.Min(pageSize, 100);

        var logsQuery = _context.AuditLogs
            .AsNoTracking()
            .Include(x => x.User)
            .AsQueryable();

        if (query.UserId.HasValue)
            logsQuery = logsQuery.Where(x => x.UserId == query.UserId.Value);

        if (!string.IsNullOrWhiteSpace(query.EntityName))
        {
            var entityName = query.EntityName.Trim().ToLower();
            logsQuery = logsQuery.Where(x => x.EntityName.ToLower().Contains(entityName));
        }

        if (query.EntityId.HasValue)
            logsQuery = logsQuery.Where(x => x.EntityId == query.EntityId.Value);

        if (query.Action.HasValue)
            logsQuery = logsQuery.Where(x => x.Action == query.Action.Value);

        if (query.FromDateUtc.HasValue)
            logsQuery = logsQuery.Where(x => x.CreatedAtUtc >= query.FromDateUtc.Value);

        if (query.ToDateUtc.HasValue)
            logsQuery = logsQuery.Where(x => x.CreatedAtUtc <= query.ToDateUtc.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            logsQuery = logsQuery.Where(x =>
                x.EntityName.ToLower().Contains(search) ||
                x.Action.ToString().ToLower().Contains(search) ||
                (x.OldValues != null && x.OldValues.ToLower().Contains(search)) ||
                (x.NewValues != null && x.NewValues.ToLower().Contains(search)) ||
                (x.User.Email != null && x.User.Email.ToLower().Contains(search)));
        }

        var totalCount = await logsQuery.CountAsync(cancellationToken);

        var items = await logsQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLog>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AuditLog?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}