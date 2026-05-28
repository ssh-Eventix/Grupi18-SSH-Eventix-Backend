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

    private static AuditLogDTO MapToDto(AuditLog log)
    {
        return new AuditLogDTO
        {
            Id = log.Id,
            UserId = log.UserId,
            UserEmail = log.User?.Email,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            Action = log.Action.ToString(),
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            CreatedAtUtc = log.CreatedAtUtc
        };
    }
}