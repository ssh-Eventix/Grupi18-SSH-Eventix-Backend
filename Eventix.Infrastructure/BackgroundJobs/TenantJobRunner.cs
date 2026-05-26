using Eventix.Application.Interfaces.Common;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.BackgroundJobs;

public class TenantJobRunner
{
    private readonly PublicDbContext _publicDbContext;
    private readonly IServiceScopeFactory _scopeFactory;

    public TenantJobRunner(
        PublicDbContext publicDbContext,
        IServiceScopeFactory scopeFactory)
    {
        _publicDbContext = publicDbContext;
        _scopeFactory = scopeFactory;
    }

    public async Task RunBookingCleanup()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<BookingCleanupJob>().Cleanup());
    }

    public async Task RunNotificationReminder()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<NotificationReminderJob>().SendEventReminders());
    }

    public async Task RunTicketExpiration()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<TicketExpirationJob>().ExpireOldTickets());
    }

    public async Task RunPaymentRetry()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<PaymentRetryJob>().RetryFailedPayments());
    }

    public async Task RunReviewReminder()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<ReviewReminderJob>().SendReviewReminders());
    }

    public async Task RunEventStatusUpdate()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<EventStatusUpdateJob>().UpdateEventStatuses());
    }

    public async Task RunCheckInAnalytics()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<CheckInAnalyticsJob>().GenerateStats());
    }

    public async Task RunArchiveEvents()
    {
        await RunForAllTenants(async sp =>
            await sp.GetRequiredService<ArchiveEventsJob>().ArchiveFinishedEvents());
    }

    private async Task RunForAllTenants(
    Func<IServiceProvider, Task> job)
    {
        var tenants = await _publicDbContext.Tenants
            .Where(x => x.IsActive)
            .ToListAsync();

        foreach (var tenant in tenants)
        {
            try
            {
                using var scope =
                    _scopeFactory.CreateScope();

                var tenantContext =
                    scope.ServiceProvider
                        .GetRequiredService<ITenantContext>();

                tenantContext.TenantId = tenant.Id;
                tenantContext.SchemaName = tenant.SchemaName;

                await job(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Tenant failed: {tenant.SchemaName}");

                Console.WriteLine(ex.Message);
            }
        }
    }
}