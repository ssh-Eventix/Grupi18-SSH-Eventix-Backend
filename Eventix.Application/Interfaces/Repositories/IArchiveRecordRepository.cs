using Eventix.Domain.Entities;

namespace Eventix.Application.Interfaces.Repositories;

public interface IArchiveRecordRepository
{
    Task<List<ArchiveRecord>> GetAllAsync();
    Task<ArchiveRecord?> GetByIdAsync(Guid id);
    Task<List<ArchiveRecord>> GetByEntityAsync(string entityName);
    Task<List<ArchiveRecord>> GetByYearAsync(int year);
    Task AddAsync(ArchiveRecord archiveRecord);
    Task DeleteAsync(ArchiveRecord archiveRecord);
    Task SaveChangesAsync();
}