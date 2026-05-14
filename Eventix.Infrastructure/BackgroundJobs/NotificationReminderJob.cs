using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.BackgroundJobs;

public class NotificationReminderJob
{
    public Task SendReminders()
    {
        Console.WriteLine("Sending notifications...");
        return Task.CompletedTask;
    }
    private readonly TenantDbContext _context;

    public NotificationReminderJob(TenantDbContext context)
    {
        _context = context;
    }

    public async Task SendEventReminders()
    {
        var tomorrow = DateTime.UtcNow.AddDays(1);

        var upcomingEvents = await _context.Events
            .Include(x => x.Bookings)
            .Where(x =>
                x.StartUtc.Date == tomorrow.Date &&
                x.Status == EventStatus.Published)
            .ToListAsync();

        foreach (var ev in upcomingEvents)
        {
            foreach (var booking in ev.Bookings)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    TenantId = booking.TenantId,
                    UserId = booking.UserId,
                    EventId = ev.Id,
                    Type = NotificationType.Reminder,
                    Title = "Event Reminder",
                    Message = $"Your event '{ev.Title}' starts tomorrow.",
                    SentAt = DateTime.UtcNow
                };

                await _context.Notifications.AddAsync(notification);
            }
        }

        await _context.SaveChangesAsync();
    }
}