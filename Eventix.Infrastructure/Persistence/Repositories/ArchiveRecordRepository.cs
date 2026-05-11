using Eventix.Application.Interfaces.Repositories;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class ArchiveRecordRepository : IArchiveRecordRepository
{
    private readonly PublicDbContext _context;

    public ArchiveRecordRepository(PublicDbContext context)
    {
        _context = context;
    }

    public async Task<List<ArchiveRecord>> GetAllAsync()
    {
        return await _context.ArchiveRecords
            .OrderByDescending(x => x.ArchivedAtUtc)
            .ToListAsync();
    }

    public async Task<ArchiveRecord?> GetByIdAsync(Guid id)
    {
        return await _context.ArchiveRecords
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ArchiveRecord>> GetByEntityAsync(string entityName)
    {
        return await _context.ArchiveRecords
            .Where(x => x.EntityName.ToLower() == entityName.ToLower())
            .OrderByDescending(x => x.ArchivedAtUtc)
            .ToListAsync();
    }

    public async Task<List<ArchiveRecord>> GetByYearAsync(int year)
    {
        return await _context.ArchiveRecords
            .Where(x => x.ArchiveYear == year)
            .OrderByDescending(x => x.ArchivedAtUtc)
            .ToListAsync();
    }

    public async Task AddAsync(ArchiveRecord archiveRecord)
    {
        await _context.ArchiveRecords.AddAsync(archiveRecord);
    }

    public Task DeleteAsync(ArchiveRecord archiveRecord)
    {
        _context.ArchiveRecords.Remove(archiveRecord);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}