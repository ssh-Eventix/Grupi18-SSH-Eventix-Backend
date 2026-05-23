using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class NotificationReminderJob
{
   
    private readonly TenantDbContext _context;

    public NotificationReminderJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task SendEventReminders()
    {
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        var events = await _context.Events
            .Include(x => x.Bookings)
            .Where(x =>
                x.Status == EventStatus.Published &&
                x.StartUtc.Date == tomorrow)
            .ToListAsync();

        foreach (var ev in events)
        {
            foreach (var booking in ev.Bookings)
            {
                bool alreadySent = await _context.Notifications.AnyAsync(x =>
                    x.UserId == booking.UserId &&
                    x.EventId == ev.Id &&
                    x.Type == NotificationType.Reminder &&
                    x.Title == "Event Reminder");

                if (alreadySent)
                    continue;

                _context.Notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    TenantId = booking.TenantId,
                    UserId = booking.UserId,
                    EventId = ev.Id,
                    Type = NotificationType.Reminder,
                    Title = "Event Reminder",
                    Message = $"Your event '{ev.Title}' starts tomorrow.",
                    SentAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}