using Eventix.Application.DTOs.Booking;
using Eventix.Application.DTOs.Ticket;
using Eventix.Application.Interfaces.Common;
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
        private readonly IPublicUserRepository _publicUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITenantContext _tenantContext;

        public BookingService(
            IBookingRepository bookingRepository,
            ITicketTypeRepository ticketTypeRepository,
            IEventRepository eventRepository,
            IPublicUserRepository publicUserRepository,
            IUserRepository userRepository,
            ITenantContext tenantContext)
        {
            _bookingRepository = bookingRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _eventRepository = eventRepository;
            _publicUserRepository = publicUserRepository;
            _userRepository = userRepository;
            _tenantContext = tenantContext;
        }

        public async Task<List<BookingDto>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return await MapBookingsAsync(bookings);
        }

        public async Task<BookingDto> GetByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetWithItemsAsync(id);

            if (booking == null)
                throw new Exception("Booking not found");

            return await MapBookingAsync(booking);
        }
        public async Task<List<BookingDto>> GetUserBookings(Guid userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return await MapBookingsAsync(bookings);
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
                TenantId = _tenantContext.TenantId,
                UserId = request.UserId,
                EventId = request.EventId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Pending,
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
                    TenantId = _tenantContext.TenantId,
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
                        TenantId = _tenantContext.TenantId,
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
            if (booking.TotalAmount == 0)
            {
                booking.Status = BookingStatus.Confirmed;
            }

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return await MapBookingAsync(booking);
        }

        public async Task<bool> UpdateBookingStatus(Guid id, UpdateBookingStatusRequest request)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if(booking == null || booking.IsDeleted)
                return false;

            if (!Enum.TryParse<BookingStatus>(request.Status, true, out var status))
                { return false; }

            if (ReleasesStock(status) && !ReleasesStock(booking.Status))
            {
                foreach (var item in booking.BookingItems)
                {
                    var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId);

                    if (ticketType == null)
                        continue;

                    ticketType.QuantityAvailable += item.Quantity;
                    ticketType.SoldQuantity -= item.Quantity;

                    if (ticketType.SoldQuantity < 0)
                        ticketType.SoldQuantity = 0;

                    _ticketTypeRepository.Update(ticketType);

                    foreach (var ticket in item.Tickets)
                    {
                        ticket.Status = status == BookingStatus.Refunded
                            ? TicketStatus.Refunded
                            : TicketStatus.Cancelled;
                    }
                }
            }
            
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

        private async Task<BookingDto> MapBookingAsync(Booking booking)
        {
            var tickets = booking.BookingItems
                .SelectMany(bi => bi.Tickets)
                .ToList();
            var buyerEmail = await ResolveBuyerEmailAsync(booking);

            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                EventId = booking.EventId,
                ReferenceNumber = booking.ReferenceNumber,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status.ToString(),
                EventTitle = booking.Event?.Title,
                BuyerEmail = buyerEmail,
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
                        UsedAt = t.UsedAt,
                        BookingId = booking.Id,
                        EventId = booking.EventId,
                        EventTitle = booking.Event?.Title,
                        BuyerEmail = buyerEmail,
                        ReferenceNumber = booking.ReferenceNumber,
                        BookingStatus = booking.Status.ToString()
                    }).ToList()
            };
        }

        private async Task<List<BookingDto>> MapBookingsAsync(List<Booking> bookings)
        {
            var result = new List<BookingDto>();

            foreach (var booking in bookings)
            {
                result.Add(await MapBookingAsync(booking));
            }

            return result;
        }

        private static bool ReleasesStock(BookingStatus status)
        {
            return status == BookingStatus.Cancelled || status == BookingStatus.Refunded;
        }

        private async Task<string?> ResolveBuyerEmailAsync(Booking booking)
        {
            if (!string.IsNullOrWhiteSpace(booking.User?.Email))
                return booking.User.Email;

            var publicUser = await _publicUserRepository.GetByIdAsync(booking.UserId, CancellationToken.None);
            if (!string.IsNullOrWhiteSpace(publicUser?.Email))
                return publicUser.Email;

            var tenantUser = await _userRepository.GetByIdAsync(booking.UserId, CancellationToken.None);
            if (!string.IsNullOrWhiteSpace(tenantUser?.Email))
                return tenantUser.Email;

            if (tenantUser?.PublicUserId is Guid publicUserId)
            {
                publicUser = await _publicUserRepository.GetByIdAsync(publicUserId, CancellationToken.None);
                return publicUser?.Email;
            }

            return null;
        }

    }
}
