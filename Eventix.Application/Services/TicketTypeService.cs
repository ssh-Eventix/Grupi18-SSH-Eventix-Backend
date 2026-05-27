using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Application.DTOs.TicketType;

namespace Eventix.Application.Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IEventSectionRepository _eventSectionRepository;

        public TicketTypeService(
            ITicketTypeRepository ticketTypeRepository,
            IEventSectionRepository eventSectionRepository)
        {
            _ticketTypeRepository = ticketTypeRepository;
            _eventSectionRepository = eventSectionRepository;
        }
        public async Task<TicketType> CreateAsync(CreateTicketTypeDto dto, Guid tenantId)
        {
            var saleStartUtc = NormalizeUtc(dto.SaleStartDate);
            var saleEndUtc = NormalizeUtc(dto.SaleEndDate);
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Ticket type name is required.");

            if (dto.Price < 0)
                throw new InvalidOperationException("Ticket price cannot be negative.");

            if (dto.QuantityAvailable <= 0)
                throw new InvalidOperationException("Ticket quantity must be greater than zero.");

            if (saleEndUtc <= saleStartUtc)
                throw new InvalidOperationException("Sale end date must be after sale start date.");

            var eventSection = await _eventSectionRepository.GetByIdAsync(dto.EventSectionId);

            if (eventSection == null)
                throw new InvalidOperationException("Event section not found.");

            if (eventSection.EventId != dto.EventId)
                throw new InvalidOperationException("Event section does not belong to the selected event.");

            var existingTicketTypes = await _ticketTypeRepository.GetByEventIdAsync(dto.EventId);
            var duplicateName = existingTicketTypes.Any(x =>
                string.Equals(x.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));

            if (duplicateName)
                throw new InvalidOperationException("A ticket type with this name already exists for this event.");

            var existingQuantityForSection = existingTicketTypes
                .Where(x => x.EventSectionId == dto.EventSectionId)
                .Sum(x => x.QuantityAvailable + x.SoldQuantity);

            if (existingQuantityForSection + dto.QuantityAvailable > eventSection.Capacity)
            {
                throw new InvalidOperationException(
                    $"Ticket quantity exceeds event section capacity. Section capacity is {eventSection.Capacity}, already allocated {existingQuantityForSection}.");
            }

            var ticketType = new TicketType
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                EventId = dto.EventId,
                EventSectionId = dto.EventSectionId,
                Name = name,
                Price = dto.Price,
                QuantityAvailable = dto.QuantityAvailable,
                SoldQuantity = 0,
                SaleStartDate = saleStartUtc,
                SaleEndDate = saleEndUtc
            };

            await _ticketTypeRepository.AddAsync(ticketType);
            await _ticketTypeRepository.SaveChangesAsync();

            return ticketType;
        }

        private static DateTime NormalizeUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
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
