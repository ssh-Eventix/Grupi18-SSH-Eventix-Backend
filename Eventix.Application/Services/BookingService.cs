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

        public BookingService(IBookingRepository bookingRepository, ITicketTypeRepository ticketTypeRepository)
        {
            _bookingRepository = bookingRepository;
            _ticketTypeRepository = ticketTypeRepository;
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
            var booking = new Booking
            {
                UserId = request.UserId,
                EventId = request.EventId,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Confirmed,
                ReferenceNumber = Guid.NewGuid().ToString()
            };
                
            decimal total = 0;

            foreach (var item in request.BookingItems)
            {
                var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId);

                if (ticketType == null)
                    throw new Exception("TicketType not found");

                if (ticketType.QuantityAvailable < item.Quantity)
                    throw new Exception("Not enough tickets available");

                var bookingItem = new BookingItem
                {
                    BookingId = booking.Id,
                    TicketTypeId = item.TicketTypeId,
                    Quantity = item.Quantity,
                    UnitPrice = ticketType.Price
                };

                for (int i = 0; i < item.Quantity; i++)
                {
                    bookingItem.Tickets.Add(new Ticket
                    {
                        TicketCode = Guid.NewGuid().ToString(),
                        QRCode = Guid.NewGuid().ToString(),
                        Status = TicketStatus.Active,
                        IssuedAt = DateTime.UtcNow
                    });
                }

                booking.TotalAmount = total;

                await _bookingRepository.AddAsync(booking);
                await _bookingRepository.SaveChangesAsync();

                return MapBooking(booking);
            }

            booking.TotalAmount = total;

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return MapBooking(booking);
        }

        private static BookingDto MapBooking(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                ReferenceNumber = booking.ReferenceNumber,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status.ToString(),
                Tickets = booking.BookingItems
                    .SelectMany(bi => bi.Tickets)
                    .Select(t => new TicketDto
                    {
                        TicketCode = t.TicketCode,
                        QRCode = t.QRCode
                    }).ToList()
            };
        }

        private static List<BookingDto> MapBookings(List<Booking> bookings)
        {
            return bookings.Select(MapBooking).ToList();
        }

    }
}
