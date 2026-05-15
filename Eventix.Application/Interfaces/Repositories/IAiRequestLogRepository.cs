using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.Application.Interfaces.Repositories
{
    public interface IAIRequestLogRepository
    {
        Task<List<AIRequestLog>> GetAllAsync();

        Task<AIRequestLog?> GetByIdAsync(Guid id);

        Task<List<AIRequestLog>> GetByUserIdAsync(Guid userId);

        Task<List<AIRequestLog>> GetByStatusAsync(AIRequestStatus status);

        Task<List<AIRequestLog>> GetByRequestTypeAsync(AIRequestType requestType);

        Task AddAsync(AIRequestLog aiRequestLog);

        void Update(AIRequestLog aiRequestLog);

        void Delete(AIRequestLog aiRequestLog);

        Task SaveChangesAsync();
    }
}