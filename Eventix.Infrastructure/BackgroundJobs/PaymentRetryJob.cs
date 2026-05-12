using Eventix.Domain.Enums;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.Infrastructure.BackgroundJobs;

public class PaymentRetryJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PaymentRetryJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task RetryFailedPayments()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var failedPayments = await context.Payments
            .Where(x => x.Status == PaymentStatus.Failed)
            .ToListAsync();

        foreach (var payment in failedPayments)
        {
            payment.Status = PaymentStatus.Pending;
        }

        await context.SaveChangesAsync();
    }
}