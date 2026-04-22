using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Repositories;

public interface ISpeakerRepository
{
    Task<List<Speaker>> GetAllAsync(CancellationToken cancellationToken);
    Task<Speaker?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Speaker speaker, CancellationToken cancellationToken);

    void Update(Speaker speaker);
    void Delete(Speaker speaker);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}