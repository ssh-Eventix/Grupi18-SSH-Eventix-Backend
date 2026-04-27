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

        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureUserRole(modelBuilder);

        ConfigureVenue(modelBuilder);
        ConfigureVenueSection(modelBuilder);

        ConfigureEventCategory(modelBuilder);
        ConfigureEvent(modelBuilder);
        ConfigureEventSection(modelBuilder);
        ConfigureEventSession(modelBuilder);
        ConfigureSpeaker(modelBuilder);

        ConfigureTicketType(modelBuilder);
        ConfigureBooking(modelBuilder);
        ConfigureBookingItem(modelBuilder);
        ConfigureTicket(modelBuilder);
        ConfigureCheckIn(modelBuilder);

        ConfigureNotification(modelBuilder);
        ConfigureDiscountCoupon(modelBuilder);
        ConfigureReview(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Email })
                .IsUnique();

            entity.HasMany(x => x.Bookings)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Name })
                .IsUnique();
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.AssignedAt)
                .HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.UserId, x.RoleId })
                .IsUnique();
        });
    }

    private static void ConfigureVenue(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("Venues");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.AddressLine1).HasMaxLength(200).IsRequired();
            entity.Property(x => x.City).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Country).HasMaxLength(100).IsRequired();

            entity.Property(x => x.IsIndoor).HasDefaultValue(true);
            entity.Property(x => x.IsAccessible).HasDefaultValue(true);

            entity.HasMany(x => x.Sections)
                .WithOne(x => x.Venue)
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

            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();

            entity.Property(x => x.DefaultBasePrice)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.SeatType)
                .HasConversion<int>();

            entity.Property(x => x.DisplayOrder)
                .HasDefaultValue(0);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.Venue)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.EventSections)
                .WithOne(x => x.VenueSection)
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

            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Icon).HasMaxLength(250);

            entity.Property(x => x.DisplayOrder).HasDefaultValue(0);
            entity.Property(x => x.IsActive).HasDefaultValue(true);

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

            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.OrganizerName).HasMaxLength(150);
            entity.Property(x => x.BannerImageUrl).HasMaxLength(500);
            entity.Property(x => x.Currency).HasMaxLength(10).IsRequired();

            entity.Property(x => x.Status).HasConversion<int>();
            entity.Property(x => x.Visibility).HasConversion<int>();

            entity.Property(x => x.MaxTicketsPerOrder).HasDefaultValue(10);
            entity.Property(x => x.MinTicketsPerOrder).HasDefaultValue(1);
            entity.Property(x => x.IsFree).HasDefaultValue(false);
            entity.Property(x => x.IsPublished).HasDefaultValue(false);

            entity.HasOne(x => x.Venue)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EventCategory)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Slug })
                .IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Event_MaxTickets", "\"MaxTicketsPerOrder\" >= \"MinTicketsPerOrder\"");
                t.HasCheckConstraint("CK_Event_DateRange", "\"EndUtc\" > \"StartUtc\"");
            });
        });
    }

    private static void ConfigureEventSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventSection>(entity =>
        {
            entity.ToTable("EventSections");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Price).HasColumnType("numeric(18,2)");
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.VenueSection)
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.VenueSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.Code })
                .IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_EventSection_Capacity", "\"Capacity\" >= 0");
                t.HasCheckConstraint("CK_EventSection_Price", "\"Price\" >= 0");
            });
        });
    }

    private static void ConfigureEventSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventSession>(entity =>
        {
            entity.ToTable("EventSessions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Speaker)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.SpeakerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.EventId);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_EventSession_DateRange", "\"EndTime\" > \"StartTime\"");
            });
        });
    }

    private static void ConfigureSpeaker(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Speaker>(entity =>
        {
            entity.ToTable("Speakers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Bio)
                .HasMaxLength(1000);

            entity.Property(x => x.Email)
                .HasMaxLength(200);

            entity.Property(x => x.Phone)
                .HasMaxLength(50);

            entity.Property(x => x.ProfileImageUrl)
                .HasMaxLength(500);

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Email });
        });
    }

    private static void ConfigureTicketType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.ToTable("TicketTypes");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Price).HasColumnType("numeric(18,2)");
            entity.Property(x => x.SoldQuantity).HasDefaultValue(0);

            entity.HasOne(x => x.Event)
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.EventId);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.Name })
                .IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_TicketType_Price", "\"Price\" >= 0");
                t.HasCheckConstraint("CK_TicketType_QuantityAvailable", "\"QuantityAvailable\" >= 0");
                t.HasCheckConstraint("CK_TicketType_SoldQuantity", "\"SoldQuantity\" >= 0");
                t.HasCheckConstraint("CK_TicketType_SoldNotGreaterThanAvailable", "\"SoldQuantity\" <= \"QuantityAvailable\"");
                t.HasCheckConstraint("CK_TicketType_SaleDateRange", "\"SaleEndDate\" > \"SaleStartDate\"");
            });
        });
    }

    private static void ConfigureBooking(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ReferenceNumber)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.TotalAmount)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.HasOne(x => x.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.EventId);

            entity.HasIndex(x => new { x.TenantId, x.ReferenceNumber })
                .IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Booking_TotalAmount", "\"TotalAmount\" >= 0");
            });
        });
    }

    private static void ConfigureBookingItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.ToTable("BookingItems");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UnitPrice)
                .HasColumnType("numeric(18,2)");

            entity.HasOne(x => x.Booking)
                .WithMany(b => b.BookingItems)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.TicketType)
                .WithMany(t => t.BookingItems)
                .HasForeignKey(x => x.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.BookingId);
            entity.HasIndex(x => x.TicketTypeId);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_BookingItem_Quantity", "\"Quantity\" > 0");
                t.HasCheckConstraint("CK_BookingItem_UnitPrice", "\"UnitPrice\" >= 0");
            });
        });
    }

    private static void ConfigureTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Tickets");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TicketCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.QRCode)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.HasOne(x => x.BookingItem)
                .WithMany(bi => bi.Tickets)
                .HasForeignKey(x => x.BookingItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.BookingItemId);

            entity.HasIndex(x => new { x.TenantId, x.TicketCode })
                .IsUnique();
        });
    }

    private static void ConfigureCheckIn(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.ToTable("CheckIns");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Notes)
                .HasMaxLength(1000);

            entity.HasOne(x => x.Ticket)
                .WithOne()
                .HasForeignKey<CheckIn>(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CheckedInByUser)
                .WithMany()
                .HasForeignKey(x => x.CheckedInByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.TicketId);

            entity.HasIndex(x => new { x.TenantId, x.TicketId })
                .IsUnique();
        });
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Message)
                .IsRequired();

            entity.Property(x => x.IsRead)
                .HasDefaultValue(false);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Event)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.EventId);
        });
    }

    private static void ConfigureDiscountCoupon(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountCoupon>(entity =>
        {
            entity.ToTable("DiscountCoupons");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DiscountValue)
                .HasColumnType("numeric(18,2)");

            entity.HasIndex(x => x.TenantId);

            entity.HasIndex(x => new { x.TenantId, x.Code })
                .IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_DiscountCoupon_DiscountValue", "\"DiscountValue\" >= 0");
            });
        });
    }

    private static void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Comment)
                .HasMaxLength(1000);

            entity.HasOne(x => x.Event)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.EventId);
            entity.HasIndex(x => x.UserId);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.UserId })
                .IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Review_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
            });
        });
    }
}