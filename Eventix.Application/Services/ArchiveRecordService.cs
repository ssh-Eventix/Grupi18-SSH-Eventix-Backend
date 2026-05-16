using Eventix.Application.DTOs.Archive;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class ArchiveRecordService : IArchiveRecordService
{
    private readonly IArchiveRecordRepository _repository;

    public ArchiveRecordService(IArchiveRecordRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ArchiveRecordResponseDTO>> GetAllAsync()
    {
        var records = await _repository.GetAllAsync();
        return records.Select(MapToDto).ToList();
    }

    public async Task<ArchiveRecordResponseDTO?> GetByIdAsync(Guid id)
    {
        var record = await _repository.GetByIdAsync(id);
        return record == null ? null : MapToDto(record);
    }

    public async Task<List<ArchiveRecordResponseDTO>> GetByEntityAsync(string entityName)
    {
        var records = await _repository.GetByEntityAsync(entityName);
        return records.Select(MapToDto).ToList();
    }

    public async Task<List<ArchiveRecordResponseDTO>> GetByYearAsync(int year)
    {
        var records = await _repository.GetByYearAsync(year);
        return records.Select(MapToDto).ToList();
    }

    public async Task<ArchiveRecordResponseDTO> CreateAsync(CreateArchiveRecordDTO dto)
    {
        var record = new ArchiveRecord
        {
            TenantId = dto.TenantId,
            SchemaName = dto.SchemaName,
            EntityName = dto.EntityName,
            EntityId = dto.EntityId,
            ArchiveYear = dto.ArchiveYear,
            DataJson = dto.DataJson,
            ArchivedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(record);
        await _repository.SaveChangesAsync();

        return MapToDto(record);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var record = await _repository.GetByIdAsync(id);

        if (record == null)
            return false;

        await _repository.DeleteAsync(record);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static ArchiveRecordResponseDTO MapToDto(ArchiveRecord record)
    {
        return new ArchiveRecordResponseDTO
        {
            Id = record.Id,
            TenantId = record.TenantId,
            SchemaName = record.SchemaName,
            EntityName = record.EntityName,
            EntityId = record.EntityId,
            ArchiveYear = record.ArchiveYear,
            DataJson = record.DataJson,
            ArchivedAtUtc = record.ArchivedAtUtc
        };
    }
}