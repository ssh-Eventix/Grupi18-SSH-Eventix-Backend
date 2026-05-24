using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class ReviewReminderJob
{
    private readonly TenantDbContext _context;

    public ReviewReminderJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task SendReviewReminders()
    {
        var now = DateTime.UtcNow;
        var since = now.AddDays(-1);

        var events = await _context.Events
            .Include(x => x.Bookings)
            .Where(x =>
                x.Status == EventStatus.Completed &&
                x.EndUtc <= now &&
                x.EndUtc >= since)
            .ToListAsync();

        foreach (var ev in events)
        {
            foreach (var booking in ev.Bookings)
            {
                bool alreadyReviewed = await _context.Reviews.AnyAsync(x =>
                    x.EventId == ev.Id &&
                    x.UserId == booking.UserId);

                bool alreadyNotified = await _context.Notifications.AnyAsync(x =>
                    x.UserId == booking.UserId &&
                    x.EventId == ev.Id &&
                    x.Title == "Leave a Review");

                if (alreadyReviewed || alreadyNotified)
                    continue;

                _context.Notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    TenantId = booking.TenantId,
                    UserId = booking.UserId,
                    EventId = ev.Id,
                    Type = NotificationType.Reminder,
                    Title = "Leave a Review",
                    Message = $"Please review the event '{ev.Title}'.",
                    SentAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}