using Hangfire;

namespace Eventix.Infrastructure.BackgroundJobs;

public static class JobScheduler
{
    public static void RegisterJobs()
    {
        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "booking-cleanup",
            runner => runner.RunBookingCleanup(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "notification-reminder",
            runner => runner.RunNotificationReminder(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "ticket-expiration",
            runner => runner.RunTicketExpiration(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "payment-retry",
            runner => runner.RunPaymentRetry(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "review-reminder",
            runner => runner.RunReviewReminder(),
            Cron.Daily);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "event-status-update",
            runner => runner.RunEventStatusUpdate(),
            Cron.Hourly);

        RecurringJob.AddOrUpdate<TenantJobRunner>(
            "checkin-analytics",
            runner => runner.RunCheckInAnalytics(),
            Cron.Hourly);
    }
}