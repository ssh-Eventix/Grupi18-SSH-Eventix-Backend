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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        ConfigureVenue(modelBuilder);
        ConfigureVenueSection(modelBuilder);
        ConfigureEventCategory(modelBuilder);
        ConfigureEvent(modelBuilder);
        ConfigureEventSection(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureVenue(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("Venues");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.AddressLine1)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.IsIndoor)
                .HasDefaultValue(true);

            entity.Property(x => x.IsAccessible)
                .HasDefaultValue(true);

            entity.HasMany(x => x.Sections)
                .WithOne()
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Code })
                .IsUnique();
        });
    }

    private static void ConfigureVenueSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VenueSection>(entity =>
        {
            entity.ToTable("VenueSections");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DefaultBasePrice)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.SeatType)
                .HasConversion<int>();

            entity.Property(x => x.DisplayOrder)
                .HasDefaultValue(0);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasMany(x => x.EventSections)
                .WithOne()
                .HasForeignKey(x => x.VenueSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.VenueId, x.Code })
                .IsUnique();
        });
    }

    private static void ConfigureEventCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventCategory>(entity =>
        {
            entity.ToTable("EventCategories");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.Icon)
                .HasMaxLength(250);

            entity.Property(x => x.DisplayOrder)
                .HasDefaultValue(0);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasMany(x => x.Events)
                .WithOne(x => x.EventCategory)
                .HasForeignKey(x => x.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Name })
                .IsUnique();
        });
    }

    private static void ConfigureEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Events");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Slug)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(4000);

            entity.Property(x => x.OrganizerName)
                .HasMaxLength(150);

            entity.Property(x => x.BannerImageUrl)
                .HasMaxLength(500);

            entity.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.Property(x => x.Visibility)
                .HasConversion<int>();

            entity.Property(x => x.MaxTicketsPerOrder)
                .HasDefaultValue(10);

            entity.Property(x => x.MinTicketsPerOrder)
                .HasDefaultValue(1);

            entity.Property(x => x.IsFree)
                .HasDefaultValue(false);

            entity.Property(x => x.IsPublished)
                .HasDefaultValue(false);

            entity.HasOne(x => x.Venue)
                .WithMany()
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EventCategory)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Slug })
                .IsUnique();
        });
    }

    private static void ConfigureEventSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventSection>(entity =>
        {
            entity.ToTable("EventSections");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Price)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne<Event>()
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<VenueSection>()
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.VenueSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.Code })
                .IsUnique();
        });
    }
}