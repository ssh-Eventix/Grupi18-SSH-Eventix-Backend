using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Application.DTOs.Ticket;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using System.Text;
using System.Text.Json;

namespace Eventix.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IPublicUserRepository _publicUserRepository;
        private readonly IUserRepository _userRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            IPublicUserRepository publicUserRepository,
            IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _publicUserRepository = publicUserRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Ticket>> GenerateTicketsAsync(Guid bookingItemId, int quantity)
        {
            var tickets = new List<Ticket>();

            for (int i = 0; i < quantity; i++)
            {
                var ticketCode = GenerateTicketCode();

                var payload = new
                {
                    BookingItemId = bookingItemId,
                    TicketCode = ticketCode,
                    IssuedAt = DateTime.UtcNow
                };

                var qrCode = GenerateQrCode(payload);

                tickets.Add(new Ticket
                {
                    BookingItemId = bookingItemId,
                    TicketCode = ticketCode,
                    QRCode = qrCode,
                    Status = TicketStatus.Active,
                    IssuedAt = DateTime.UtcNow
                });
            }

            await _ticketRepository.AddRangeAsync(tickets);
            await _ticketRepository.SaveChangesAsync();

            return tickets;
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _ticketRepository.GetByIdAsync(id);
        }

        public async Task<TicketDto?> GetDtoByIdAsync(Guid id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            return ticket == null ? null : await MapTicketAsync(ticket);
        }

        public async Task<List<TicketDto>> GetAllAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return await MapTicketsAsync(tickets);
        }

        public async Task<Ticket?> GetByCodeAsync(string ticketCode)
        {
            return await _ticketRepository.GetByCodeAsync(ticketCode);
        }

        public async Task<TicketDto?> GetDtoByCodeAsync(string ticketCode)
        {
            var ticket = await _ticketRepository.GetByCodeAsync(ticketCode);
            return ticket == null ? null : await MapTicketAsync(ticket);
        }

        public async Task<bool> ValidateTicketAsync(string ticketCode)
        {
            var ticket = await _ticketRepository.GetByCodeAsync(ticketCode);

            if (ticket == null)
                return false;

            if (ticket.Status != TicketStatus.Active)
                return false;

            return true;
        }

        public async Task CheckInAsync(string ticketCode)
        {
            var ticket = await _ticketRepository.GetByCodeAsync(ticketCode);

            if (ticket == null)
                throw new Exception("Ticket not found");

            if (ticket.Status == TicketStatus.Used)
                throw new Exception("Ticket already used");

            if (ticket.Status != TicketStatus.Active)
                throw new Exception("Ticket is not valid");

            ticket.Status = TicketStatus.Used;
            ticket.UsedAt = DateTime.UtcNow;

            await _ticketRepository.SaveChangesAsync();
        }

        private string GenerateTicketCode()
        {
            return $"TKT-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        private string GenerateQrCode(object data)
        {
            var json = JsonSerializer.Serialize(data);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private async Task<TicketDto> MapTicketAsync(Ticket ticket)
        {
            var booking = ticket.BookingItem?.Booking;
            var buyerEmail = booking == null ? null : await ResolveBuyerEmailAsync(booking);

            return new TicketDto
            {
                Id = ticket.Id,
                TicketCode = ticket.TicketCode,
                QRCode = ticket.QRCode,
                Status = (int)ticket.Status,
                IssuedAt = ticket.IssuedAt,
                UsedAt = ticket.UsedAt,
                BookingId = booking?.Id,
                EventId = booking?.EventId,
                EventTitle = booking?.Event?.Title,
                BuyerEmail = buyerEmail,
                ReferenceNumber = booking?.ReferenceNumber,
                BookingStatus = booking?.Status.ToString()
            };
        }

        private async Task<List<TicketDto>> MapTicketsAsync(List<Ticket> tickets)
        {
            var result = new List<TicketDto>();

            foreach (var ticket in tickets)
            {
                result.Add(await MapTicketAsync(ticket));
            }

            return result;
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
