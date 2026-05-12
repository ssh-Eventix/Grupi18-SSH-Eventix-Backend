using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class ReviewReminderJob
{
    public Task SendReminders()
    {
        Console.WriteLine("Sending review reminders...");
        return Task.CompletedTask;
    }
    private readonly TenantDbContext _context;

    public ReviewReminderJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task SendReviewReminders()
    {
        var finishedEvents = await _context.Events
            .Include(x => x.Bookings)
            .Where(x =>
                x.EndUtc < DateTime.UtcNow &&
                x.EndUtc > DateTime.UtcNow.AddDays(-1))
            .ToListAsync();

        foreach (var ev in finishedEvents)
        {
            foreach (var booking in ev.Bookings)
            {
                bool alreadyReviewed = await _context.Reviews.AnyAsync(x =>
                    x.EventId == ev.Id &&
                    x.UserId == booking.UserId);

                if (!alreadyReviewed)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        TenantId = booking.TenantId,
                        UserId = booking.UserId,
                        EventId = ev.Id,
                        Type = NotificationType.Info,
                        Title = "Leave a Review",
                        Message = $"Please review the event '{ev.Title}'.",
                        SentAt = DateTime.UtcNow
                    };

                    await _context.Notifications.AddAsync(notification);
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}