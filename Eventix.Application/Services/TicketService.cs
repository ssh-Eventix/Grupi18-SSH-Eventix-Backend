using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eventix.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
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

        public async Task<Ticket?> GetByCodeAsync(string ticketCode)
        {
            return await _ticketRepository.GetByCodeAsync(ticketCode);
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

            _ticketRepository.Update(ticket);
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
    }
}
