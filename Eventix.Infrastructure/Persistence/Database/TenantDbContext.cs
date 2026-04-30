using Eventix.Application.Interfaces.Common;
using Eventix.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Database;

public class TenantDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public string SchemaName => _tenantContext.SchemaName ?? "public";

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueSection> VenueSections => Set<VenueSection>();

    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventSection> EventSections => Set<EventSection>();

    public DbSet<EventSession> EventSessions => Set<EventSession>();
    public DbSet<Speaker> Speakers => Set<Speaker>();

    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<CheckIn> CheckIns => Set<CheckIn>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DiscountCoupon> DiscountCoupons => Set<DiscountCoupon>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureUserRole(modelBuilder);

        ConfigureVenue(modelBuilder);
        ConfigureVenueSection(modelBuilder);

        ConfigureEvent(modelBuilder);
        ConfigureEventSection(modelBuilder);

        ConfigureTicket(modelBuilder);
        ConfigureCheckIn(modelBuilder);

        ConfigureBooking(modelBuilder);
        ConfigureBookingItem(modelBuilder);

        ConfigurePayment(modelBuilder);
        ConfigurePaymentMethod(modelBuilder);

        ConfigureReview(modelBuilder);
        ConfigureDiscountCoupon(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    // ================= USERS =================

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);

            entity.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.UserId, x.RoleId }).IsUnique();
        });
    }

    // ================= VENUE =================

    private static void ConfigureVenue(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });
    }

    private static void ConfigureVenueSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VenueSection>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.VenueId, x.Code }).IsUnique();
        });
    }

    // ================= EVENTS =================

    private static void ConfigureEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique();
        });
    }

    private static void ConfigureEventSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventSection>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.EventId, x.Code }).IsUnique();
        });
    }

    // ================= TICKETS =================

    private static void ConfigureTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.TicketCode }).IsUnique();

            entity.HasIndex(x => x.QRCode).IsUnique();
        });
    }

    private static void ConfigureCheckIn(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.TicketId }).IsUnique();

            entity.HasOne(x => x.Ticket)
                .WithOne(t => t.CheckIn)
                .HasForeignKey<CheckIn>(x => x.TicketId);
        });
    }

    // ================= BOOKINGS =================

    private static void ConfigureBooking(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.ReferenceNumber }).IsUnique();
        });
    }

    private static void ConfigureBookingItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.HasCheckConstraint("CK_Quantity", "\"Quantity\" > 0");
        });
    }

    // ================= PAYMENTS =================

    private static void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasOne(x => x.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(x => x.BookingId);
        });
    }

    private static void ConfigurePaymentMethod(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });
    }

    // ================= REVIEW =================

    private static void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Review_Rating",
                    "\"Rating\" >= 1 AND \"Rating\" <= 5"
                );
            });

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.UserId }).IsUnique();
        });
    }

    // ================= COUPONS =================

    private static void ConfigureDiscountCoupon(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountCoupon>(entity =>
        {
            entity.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });
    }
}