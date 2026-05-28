using Eventix.Application.DTOs.AuditLog;
using Eventix.Application.DTOs.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AuditLogDTO>> GetPagedAsync(
        AuditLogQueryDTO query,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetPagedAsync(query, cancellationToken);

        return new PagedResult<AuditLogDTO>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<AuditLogDTO?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var log = await _repository.GetByIdAsync(id, cancellationToken);

        return log is null ? null : MapToDto(log);
    }

    public async Task CreateAsync(
        CreateAuditLogDTO dto,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            TenantName = dto.TenantName,
            UserId = dto.UserId,
            UserEmail = dto.UserEmail,
            EntityName = dto.EntityName,
            EntityId = dto.EntityId,
            Action = dto.Action,
            OldValues = dto.OldValues,
            NewValues = dto.NewValues,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(auditLog, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static AuditLogDTO MapToDto(AuditLog log)
    {
        return new AuditLogDTO
        {
            Id = log.Id,
            TenantId = log.TenantId,
            TenantName = log.TenantName,
            UserId = log.UserId,
            UserEmail = log.UserEmail,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            Action = log.Action.ToString(),
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            CreatedAtUtc = log.CreatedAtUtc
        };
    }
}