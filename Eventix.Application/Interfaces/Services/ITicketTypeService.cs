using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services
{
    public interface ITicketTypeService
    {
        Task<TicketType?> GetByIdAsync(Guid id);

        Task<List<TicketType>> GetByEventIdAsync(Guid eventId);

        Task<List<TicketType>> GetAvailableByEventIdAsync(Guid eventId);

        Task<bool> IsAvailableAsync(Guid ticketTypeId, int quantity);

        Task ReduceStockAsync(Guid ticketTypeId, int quantity);
    }
}
