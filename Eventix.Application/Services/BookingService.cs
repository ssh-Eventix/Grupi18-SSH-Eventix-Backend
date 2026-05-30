using Eventix.Application.DTOs.Booking;
using Eventix.Application.DTOs.Ticket;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

namespace Eventix.Application.Services
{
    public class BookingService : IBookingService
    {
        public readonly IBookingRepository _bookingRepository;
        public readonly ITicketTypeRepository _ticketTypeRepository;
        public readonly IEventRepository _eventRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ITicketTypeRepository ticketTypeRepository,
            IEventRepository eventRepository)
        {
            _bookingRepository = bookingRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _eventRepository = eventRepository;
        }

        public async Task<List<BookingDto>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return MapBookings(bookings);
        }

        public async Task<BookingDto> GetByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetWithItemsAsync(id);

            if (booking == null)
                throw new Exception("Booking not found");

            return MapBooking(booking);
        }
        public async Task<List<BookingDto>> GetUserBookings(Guid userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return MapBookings(bookings);
        }

        public async Task<BookingDto> CreateBooking(CreateBookingRequest request)
        {
            if (request.BookingItems == null || !request.BookingItems.Any())
                throw new Exception("Booking must have at least one item");

            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);

            if (eventEntity == null)
                throw new Exception("Event not found");

            var totalRequestedQuantity = request.BookingItems.Sum(x => x.Quantity);

            if (totalRequestedQuantity < eventEntity.MinTicketsPerOrder)
                throw new Exception($"Minimum tickets per order is {eventEntity.MinTicketsPerOrder}.");

            if (totalRequestedQuantity > eventEntity.MaxTicketsPerOrder)
                throw new Exception($"Maximum tickets per order is {eventEntity.MaxTicketsPerOrder}.");

            var booking = new Booking
            {
                UserId = request.UserId,
                EventId = request.EventId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Confirmed,
                ReferenceNumber = $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}"
            };
                
            decimal total = 0;
            var now = DateTime.UtcNow;

            foreach (var item in request.BookingItems)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Quantity must be greater than zero");

                var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId);

                if (ticketType == null)
                    throw new Exception("TicketType not found");

                if (ticketType.EventId != request.EventId)
                    throw new Exception("TicketType does not belong to this event");

                if (ticketType.SaleStartDate > now || ticketType.SaleEndDate < now)
                    throw new Exception("Ticket sales are not active for this ticket type");

                if (ticketType.QuantityAvailable < item.Quantity)
                    throw new Exception("Not enough tickets available");

                var bookingItem = new BookingItem
                {
                    TicketTypeId = item.TicketTypeId,
                    EventSectionId = ticketType.EventSectionId,
                    Quantity = item.Quantity,
                    UnitPrice = ticketType.Price
                };

                for (int i = 0; i < item.Quantity; i++)
                {
                    var ticketCode = $"TKT-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                    bookingItem.Tickets.Add(new Ticket
                    {
                        TicketCode = ticketCode,
                        QRCode = ticketCode,
                        Status = TicketStatus.Active,
                        IssuedAt = DateTime.UtcNow
                    });
                }

                ticketType.QuantityAvailable -= item.Quantity;
                ticketType.SoldQuantity += item.Quantity;

                total += item.Quantity * ticketType.Price;

                booking.BookingItems.Add(bookingItem);
            }

            booking.TotalAmount = total;

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return MapBooking(booking);
        }

        public async Task<bool> UpdateBookingStatus(Guid id, UpdateBookingStatusRequest request)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if(booking == null || booking.IsDeleted)
                return false;

            if (!Enum.TryParse<BookingStatus>(request.Status, true, out var status))
                { return false; }
            
            booking.Status = status;
            booking.UpdatedAtUtc = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBooking(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null || booking.IsDeleted)
                return false;

            booking.IsDeleted = true;
            booking.UpdatedAtUtc = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return true;
        }

        private static BookingDto MapBooking(Booking booking)
        {
            var tickets = booking.BookingItems
                .SelectMany(bi => bi.Tickets)
                .ToList();

            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                EventId = booking.EventId,
                ReferenceNumber = booking.ReferenceNumber,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status.ToString(),
                EventTitle = booking.Event?.Title,
                BuyerEmail = booking.User?.Email,
                Quantity = booking.BookingItems.Sum(bi => bi.Quantity),
                TicketCode = tickets.FirstOrDefault()?.TicketCode,
                BookingDate = booking.BookingDate,
                Tickets = booking.BookingItems
                    .SelectMany(bi => bi.Tickets)
                    .Select(t => new TicketDto
                    {
                        Id = t.Id,
                        TicketCode = t.TicketCode,
                        QRCode = t.QRCode,
                        Status = (int)t.Status,
                        IssuedAt = t.IssuedAt,
                        UsedAt = t.UsedAt
                    }).ToList()
            };
        }

        private static List<BookingDto> MapBookings(List<Booking> bookings)
        {
            return bookings.Select(MapBooking).ToList();
        }

    }
}
