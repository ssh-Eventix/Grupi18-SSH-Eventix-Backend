using Hangfire;
using Microsoft.Extensions.DependencyInjection;
namespace Eventix.Infrastructure.BackgroundJobs;

public static class JobScheduler
{
    public static void RegisterJobs()
    {
        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "booking-cleanup",
            runner => runner.RunForAllTenants(sp =>
                sp.GetRequiredService<BookingCleanupJob>().Cleanup()),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "notification-reminder",
            runner => runner.RunForAllTenants(sp =>
                sp.GetRequiredService<NotificationReminderJob>().SendEventReminders()),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "ticket-expiration",
            runner => runner.RunForAllTenants(sp =>
                sp.GetRequiredService<TicketExpirationJob>().ExpireOldTickets()),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "payment-retry",
            runner => runner.RunForAllTenants(sp =>
                sp.GetRequiredService<PaymentRetryJob>().RetryFailedPayments()),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "review-reminder",
            runner => runner.RunForAllTenants(sp =>
                sp.GetRequiredService<ReviewReminderJob>().SendReviewReminders()),
            Cron.Daily);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "event-status-update",
            runner => runner.RunForAllTenants(sp =>
                sp.GetRequiredService<EventStatusUpdateJob>().UpdateEventStatuses()),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "checkin-analytics",
            runner => runner.RunForAllTenants(sp =>
                sp.GetRequiredService<CheckInAnalyticsJob>().GenerateStats()),
            Cron.Hourly);
    }
}