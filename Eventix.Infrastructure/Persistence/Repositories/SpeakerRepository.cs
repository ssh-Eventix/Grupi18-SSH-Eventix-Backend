using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eventix.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Repositories;

public class SpeakerRepository : ISpeakerRepository
{
    private readonly PublicDbContext _context;

    public SpeakerRepository(PublicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Speaker>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<Speaker>().ToListAsync(cancellationToken);
    }

    public async Task<Speaker?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Speaker>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(Speaker speaker, CancellationToken cancellationToken)
    {
        await _context.Set<Speaker>().AddAsync(speaker, cancellationToken);
    }

    public void Update(Speaker speaker)
    {
        _context.Set<Speaker>().Update(speaker);
    }

    public void Delete(Speaker speaker)
    {
        _context.Set<Speaker>().Remove(speaker);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}