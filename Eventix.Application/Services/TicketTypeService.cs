using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventix.Application.Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly ITicketTypeRepository _ticketTypeRepository;

        public TicketTypeService(ITicketTypeRepository ticketTypeRepository)
        {
            _ticketTypeRepository = ticketTypeRepository;
        }

        public async Task<TicketType?> GetByIdAsync(Guid id)
        {
            return await _ticketTypeRepository.GetByIdAsync(id);
        }

        public async Task<List<TicketType>> GetByEventIdAsync(Guid eventId)
        {
            return await _ticketTypeRepository.GetByEventIdAsync(eventId);
        }

        public async Task<List<TicketType>> GetAvailableByEventIdAsync(Guid eventId)
        {
            return await _ticketTypeRepository
                .GetAvailableByEventIdAsync(eventId, DateTime.UtcNow);
        }

        public async Task<bool> IsAvailableAsync(Guid ticketTypeId, int quantity)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(ticketTypeId);

            if (ticketType == null)
                return false;

            var now = DateTime.UtcNow;

            if (ticketType.SaleStartDate > now || ticketType.SaleEndDate < now)
                return false;

            return ticketType.QuantityAvailable >= quantity;
        }

        public async Task ReduceStockAsync(Guid ticketTypeId, int quantity)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(ticketTypeId);

            if (ticketType == null)
                throw new Exception("TicketType not found");

            if (ticketType.QuantityAvailable < quantity)
                throw new Exception("Not enough tickets available");

            ticketType.QuantityAvailable -= quantity;
            ticketType.SoldQuantity += quantity;

            _ticketTypeRepository.Update(ticketType);
            await _ticketTypeRepository.SaveChangesAsync();
        }
    }
}
