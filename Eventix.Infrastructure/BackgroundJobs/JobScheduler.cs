using Hangfire;
using Eventix.Infrastructure.BackgroundJobs;
namespace Eventix.Infrastructure.BackgroundJobs;

public static class JobScheduler
{
    public static void RegisterJobs()
    {
        RecurringJob.AddOrUpdate<BookingCleanupJob>(
            "booking-cleanup",
            x => x.Cleanup(),
            Cron.Minutely);

        RecurringJob.AddOrUpdate<NotificationReminderJob>(
            "notification-reminder",
            x => x.SendReminders(),
            Cron.Minutely);

        RecurringJob.AddOrUpdate<TicketExpirationJob>(
            "ticket-expiration",
            x => x.ExpireTickets(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<PaymentRetryJob>(
            "payment-retry",
            x => x.RetryFailedPayments(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<ReviewReminderJob>(
            "review-reminder",
            x => x.SendReminders(),
            Cron.Daily);

        RecurringJob.AddOrUpdate<EventStatusUpdateJob>(
            "event-status-update",
            x => x.UpdateEventStatuses(),
            Cron.Minutely);

        RecurringJob.AddOrUpdate<CouponExpirationJob>(
            "coupon-expiration",
            x => x.ExpireCoupons(),
            Cron.Daily);

        RecurringJob.AddOrUpdate<CheckInAnalyticsJob>(
            "checkin-analytics",
            x => x.GenerateStats(),
            Cron.Hourly);
    }
}