using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class BookingCleanupJob
{
    private readonly TenantDbContext _context;

    public BookingCleanupJob(TenantDbContext context)
    {
        _context = context;
    }

    public Task Cleanup()
    {
        Console.WriteLine("Cleaning up bookings...");
        return RemoveExpiredBookings();
    }

    public async Task RemoveExpiredBookings()
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-15);

        var expiredBookings = await _context.Bookings
            .Include(x => x.BookingItems)
            .Where(x =>
                x.Status == BookingStatus.Pending &&
                x.BookingDate < expirationTime)
            .ToListAsync();

        foreach (var booking in expiredBookings)
        {
            booking.Status = BookingStatus.Cancelled;

            foreach (var item in booking.BookingItems)
            {
                var ticketType = await _context.TicketTypes
                    .FirstOrDefaultAsync(t => t.Id == item.TicketTypeId);

                if (ticketType == null)
                    continue;

                ticketType.QuantityAvailable += item.Quantity;
                ticketType.SoldQuantity -= item.Quantity;

                if (ticketType.SoldQuantity < 0)
                    ticketType.SoldQuantity = 0;
            }
        }

        await _context.SaveChangesAsync();
    }
}
