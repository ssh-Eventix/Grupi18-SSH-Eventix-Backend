using Eventix.Application.Interfaces.Common;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;
using Eventix.Infrastructure.BackgroundJobs;
using Eventix.Infrastructure.MultiTenancy;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Eventix.IntegrationTests.BackgroundJobs;

public class BackgroundJobIntegrationTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public BackgroundJobIntegrationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task BookingCleanupJob_Should_Cancel_Expired_Pending_Booking_And_Restore_SoldQuantity()
    {
        var tenantId = Guid.NewGuid();
        var schemaName = $"test_bg_{Guid.NewGuid():N}";

        await using var scope = _fixture.Services.CreateAsyncScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = tenantId;
        tenantContext.SchemaName = schemaName;

        var provisioner = scope.ServiceProvider.GetRequiredService<TenantSchemaProvisioner>();
        await provisioner.ProvisionTenantSchemaAsync(schemaName);

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var data = await SeedBookingDataAsync(db, tenantId);

        var job = new BookingCleanupJob(db);

        await job.RemoveExpiredBookings();

        var booking = await db.Bookings.FindAsync(data.BookingId);
        var ticketType = await db.TicketTypes.FindAsync(data.TicketTypeId);

        Assert.NotNull(booking);
        Assert.NotNull(ticketType);

        Assert.Equal(BookingStatus.Cancelled, booking!.Status);
        Assert.Equal(3, ticketType!.SoldQuantity);

        await _fixture.DropSchemaAsync(schemaName);
    }

    [Fact]
    public async Task BookingCleanupJob_Should_Not_Cancel_Recent_Pending_Booking()
    {
        var tenantId = Guid.NewGuid();
        var schemaName = $"test_bg_{Guid.NewGuid():N}";

        await using var scope = _fixture.Services.CreateAsyncScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = tenantId;
        tenantContext.SchemaName = schemaName;

        var provisioner = scope.ServiceProvider.GetRequiredService<TenantSchemaProvisioner>();
        await provisioner.ProvisionTenantSchemaAsync(schemaName);

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var data = await SeedBookingDataAsync(
            db,
            tenantId,
            bookingDate: DateTime.UtcNow.AddMinutes(-5));

        var job = new BookingCleanupJob(db);

        await job.RemoveExpiredBookings();

        var booking = await db.Bookings.FindAsync(data.BookingId);
        var ticketType = await db.TicketTypes.FindAsync(data.TicketTypeId);

        Assert.NotNull(booking);
        Assert.NotNull(ticketType);

        Assert.Equal(BookingStatus.Pending, booking!.Status);
        Assert.Equal(5, ticketType!.SoldQuantity);

        await _fixture.DropSchemaAsync(schemaName);
    }

    [Fact]
    public async Task CheckInAnalyticsJob_Should_Print_Todays_CheckIn_Count()
    {
        var tenantId = Guid.NewGuid();
        var schemaName = $"test_bg_{Guid.NewGuid():N}";

        await using var scope = _fixture.Services.CreateAsyncScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.TenantId = tenantId;
        tenantContext.SchemaName = schemaName;

        var provisioner = scope.ServiceProvider.GetRequiredService<TenantSchemaProvisioner>();
        await provisioner.ProvisionTenantSchemaAsync(schemaName);

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        await SeedCheckInDataAsync(db, tenantId);

        var job = new CheckInAnalyticsJob(db);

        var originalOut = Console.Out;

        await using var writer = new StringWriter();
        Console.SetOut(writer);

        await job.GenerateStats();

        Console.SetOut(originalOut);

        var output = writer.ToString();

        Assert.Contains("Check-ins today: 2", output);

        await _fixture.DropSchemaAsync(schemaName);
    }

    private static async Task<(Guid BookingId, Guid TicketTypeId)> SeedBookingDataAsync(
        TenantDbContext db,
        Guid tenantId,
        DateTime? bookingDate = null)
    {
        var user = CreateUser(tenantId);
        var venue = CreateVenue(tenantId);
        var category = CreateEventCategory(tenantId);

        db.Users.Add(user);
        db.Venues.Add(venue);
        db.EventCategories.Add(category);
        await db.SaveChangesAsync();

        var venueSection = CreateVenueSection(tenantId, venue.Id);
        db.VenueSections.Add(venueSection);
        await db.SaveChangesAsync();

        var ev = CreateEvent(tenantId, venue.Id, category.Id);
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        var eventSection = CreateEventSection(tenantId, ev.Id, venueSection.Id);
        db.EventSections.Add(eventSection);
        await db.SaveChangesAsync();

        var ticketType = new TicketType
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EventId = ev.Id,
            EventSectionId = eventSection.Id,
            Name = $"VIP {Guid.NewGuid():N}",
            Price = 10,
            QuantityAvailable = 10,
            SoldQuantity = 5,
            SaleStartDate = DateTime.UtcNow.AddDays(-1),
            SaleEndDate = DateTime.UtcNow.AddDays(1),
            CreatedAtUtc = DateTime.UtcNow
        };

        db.TicketTypes.Add(ticketType);
        await db.SaveChangesAsync();

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = user.Id,
            EventId = ev.Id,
            BookingDate = bookingDate ?? DateTime.UtcNow.AddMinutes(-30),
            TotalAmount = 20,
            Status = BookingStatus.Pending,
            ReferenceNumber = $"REF-{Guid.NewGuid():N}",
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        var bookingItem = new BookingItem
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            BookingId = booking.Id,
            TicketTypeId = ticketType.Id,
            EventSectionId = eventSection.Id,
            Quantity = 2,
            UnitPrice = 10,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.BookingItems.Add(bookingItem);
        await db.SaveChangesAsync();

        return (booking.Id, ticketType.Id);
    }

    private static async Task SeedCheckInDataAsync(TenantDbContext db, Guid tenantId)
    {
        var data = await SeedBookingDataAsync(db, tenantId);

        var user = await db.Users.FindAsync(
            db.Bookings.First(x => x.Id == data.BookingId).UserId);

        var bookingItem = db.BookingItems.First(x => x.BookingId == data.BookingId);

        var ticket1 = CreateTicket(tenantId, bookingItem.Id);
        var ticket2 = CreateTicket(tenantId, bookingItem.Id);
        var ticket3 = CreateTicket(tenantId, bookingItem.Id);

        db.Tickets.AddRange(ticket1, ticket2, ticket3);
        await db.SaveChangesAsync();

        db.CheckIns.AddRange(
            new CheckIn
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                TicketId = ticket1.Id,
                CheckedInByUserId = user!.Id,
                CheckInTime = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow
            },
            new CheckIn
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                TicketId = ticket2.Id,
                CheckedInByUserId = user.Id,
                CheckInTime = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow
            },
            new CheckIn
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                TicketId = ticket3.Id,
                CheckedInByUserId = user.Id,
                CheckInTime = DateTime.UtcNow.AddDays(-1),
                CreatedAtUtc = DateTime.UtcNow
            });

        await db.SaveChangesAsync();
    }

    private static User CreateUser(Guid tenantId)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FirstName = "Test",
            LastName = "User",
            Email = $"user-{Guid.NewGuid():N}@test.com",
            PasswordHash = "hashed-password",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static Venue CreateVenue(Guid tenantId)
    {
        return new Venue
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = $"Venue {Guid.NewGuid():N}",
            Code = $"VEN-{Guid.NewGuid():N}",
            AddressLine1 = "Rruga B",
            City = "Prishtina",
            Country = "Kosovo",
            TotalCapacity = 100,
            IsIndoor = true,
            IsAccessible = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static VenueSection CreateVenueSection(Guid tenantId, Guid venueId)
    {
        return new VenueSection
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            VenueId = venueId,
            Name = "Main Section",
            Code = $"SEC-{Guid.NewGuid():N}",
            Capacity = 100,
            DefaultBasePrice = 10,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static EventCategory CreateEventCategory(Guid tenantId)
    {
        return new EventCategory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = $"Category {Guid.NewGuid():N}",
            Description = "Test category",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static Event CreateEvent(Guid tenantId, Guid venueId, Guid categoryId)
    {
        return new Event
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            VenueId = venueId,
            EventCategoryId = categoryId,
            Title = "Test Event",
            Slug = $"test-event-{Guid.NewGuid():N}",
            StartUtc = DateTime.UtcNow.AddDays(1),
            EndUtc = DateTime.UtcNow.AddDays(2),
            Currency = "EUR",
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static EventSection CreateEventSection(
        Guid tenantId,
        Guid eventId,
        Guid venueSectionId)
    {
        return new EventSection
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EventId = eventId,
            VenueSectionId = venueSectionId,
            Name = "Event Section",
            Code = $"EVSEC-{Guid.NewGuid():N}",
            Capacity = 100,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static Ticket CreateTicket(Guid tenantId, Guid bookingItemId)
    {
        return new Ticket
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            BookingItemId = bookingItemId,
            TicketCode = $"TICKET-{Guid.NewGuid():N}",
            QRCode = $"QR-{Guid.NewGuid():N}",
            Status = TicketStatus.Active,
            IssuedAt = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}