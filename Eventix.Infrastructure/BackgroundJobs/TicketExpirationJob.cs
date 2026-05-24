using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class TicketExpirationJob
{
    public Task ExpireTickets()
    {
        Console.WriteLine("Expiring tickets...");
        return Task.CompletedTask;
    }
    private readonly TenantDbContext _context;

    public TicketExpirationJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task ExpireOldTickets()
    {
        var expiredTickets = await _context.Tickets
            .Include(x => x.BookingItem)
            .ThenInclude(x => x.Booking)
            .ThenInclude(x => x.Event)
            .Where(x =>
                x.Status == TicketStatus.Active &&
                x.BookingItem.Booking.Event.EndUtc < DateTime.UtcNow)
            .ToListAsync();

        foreach (var ticket in expiredTickets)
        {
            ticket.Status = TicketStatus.Cancelled;
        }

        await _context.SaveChangesAsync();
    }
}