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
    public DbSet<AIRequestLog> AIRequestLogs => Set<AIRequestLog>();

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
        ConfigureEventCategory(modelBuilder);

        ConfigureTicket(modelBuilder);
        ConfigureCheckIn(modelBuilder);
        ConfigureTicketType(modelBuilder);
        ConfigureEventSession(modelBuilder);

        ConfigureBooking(modelBuilder);
        ConfigureBookingItem(modelBuilder);
        ConfigureSpeaker(modelBuilder);

        ConfigurePayment(modelBuilder);
        ConfigurePaymentMethod(modelBuilder);
        ConfigureNotification(modelBuilder);

        ConfigureReview(modelBuilder);
        ConfigureDiscountCoupon(modelBuilder);
        ConfigureAIRequestLog(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    // ================= USERS =================

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(500);

            entity.Property(x => x.IsGlobal).HasDefaultValue(false);

            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.TenantId, x.UserId, x.RoleId }).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // ================= VENUE =================

    private static void ConfigureVenue(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("Venue", t =>
            {
                t.HasCheckConstraint("CK_Venue_TotalCapacity", "\"TotalCapacity\" >= 0");
            });

            entity.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });
    }

    private static void ConfigureVenueSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VenueSection>(entity =>
        {
            entity.ToTable("VenueSection", t =>
            {
                t.HasCheckConstraint("CK_VenueSection_Capacity", "\"Capacity\" >= 0");
                t.HasCheckConstraint("CK_VenueSection_DefaultBasePrice", "\"DefaultBasePrice\" IS NULL OR \"DefaultBasePrice\" >= 0");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Code).IsRequired().HasMaxLength(50);
            entity.Property(x => x.DefaultBasePrice).HasPrecision(18, 2);

            entity.HasIndex(x => new { x.TenantId, x.VenueId, x.Code }).IsUnique();

            entity.HasOne(x => x.Venue)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

        });
    }

    // ================= EVENTS =================

    private static void ConfigureEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event", t =>
            {
                t.HasCheckConstraint("CK_Event_DateRange", "\"EndUtc\" > \"StartUtc\"");
                t.HasCheckConstraint("CK_Event_MinTicketsPerOrder", "\"MinTicketsPerOrder\" > 0");
                t.HasCheckConstraint("CK_Event_MaxTicketsPerOrder", "\"MaxTicketsPerOrder\" >= \"MinTicketsPerOrder\"");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Slug).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Description).HasMaxLength(3000);
            entity.Property(x => x.OrganizerName).HasMaxLength(200);
            entity.Property(x => x.BannerImageUrl).HasMaxLength(500);
            entity.Property(x => x.Currency).IsRequired().HasMaxLength(3);

            entity.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique();

            entity.HasOne(x => x.Venue)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EventCategory)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        });
    }

    private static void ConfigureEventSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventSection>(entity =>
        {
            entity.ToTable("EventSection", t =>
            {
                t.HasCheckConstraint("CK_EventSection_Capacity", "\"Capacity\" >= 0");
                t.HasCheckConstraint(
                    "CK_EventSection_SalesRange",
                    "\"SalesStartUtc\" IS NULL OR \"SalesEndUtc\" IS NULL OR \"SalesEndUtc\" > \"SalesStartUtc\""
                );
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Code).IsRequired().HasMaxLength(50);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.Code }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.EventId, x.VenueSectionId }).IsUnique();

            entity.HasOne(x => x.Event)
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.VenueSection)
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.VenueSectionId)
                .OnDelete(DeleteBehavior.Restrict);

        });
    }

    private static void ConfigureEventCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventCategory>(entity =>
        {
            entity.ToTable("EventCategory");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Icon).HasMaxLength(100);

            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });
    }

    private static void ConfigureEventSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventSession>(entity =>
        {
            entity.ToTable("EventSession", t =>
            {
                t.HasCheckConstraint("CK_EventSession_TimeRange", "\"EndTime\" > \"StartTime\"");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Description).HasMaxLength(2000);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Speaker)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.SpeakerId)
                .OnDelete(DeleteBehavior.SetNull);

        });
    }

    private static void ConfigureSpeaker(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Speaker>(entity =>
        {
            entity.ToTable("Speaker");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Bio).HasMaxLength(2000);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.ProfileImageUrl).HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.Email })
                    .IsUnique()
                    .HasFilter("\"Email\" IS NOT NULL");
        });
    }

    // ================= TICKETS =================

    private static void ConfigureTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TicketCode).IsRequired().HasMaxLength(100);
            entity.Property(x => x.QRCode).IsRequired().HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.TicketCode }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.QRCode }).IsUnique();

            entity.HasOne(x => x.BookingItem)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.BookingItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTicketType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.ToTable("TicketType", t =>
            {
                t.HasCheckConstraint("CK_TicketType_Price", "\"Price\" >= 0");
                t.HasCheckConstraint("CK_TicketType_QuantityAvailable", "\"QuantityAvailable\" >= 0");
                t.HasCheckConstraint("CK_TicketType_SoldQuantity", "\"SoldQuantity\" >= 0");
                t.HasCheckConstraint("CK_TicketType_SaleRange", "\"SaleStartDate\" IS NULL OR \"SaleEndDate\" IS NULL OR \"SaleEndDate\" > \"SaleStartDate\"");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Price).HasPrecision(18, 2);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.Name }).IsUnique();

            entity.HasOne(x => x.Event)
                .WithMany(x => x.TicketTypes)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.EventSection)
                .WithMany(x => x.TicketTypes)
                .HasForeignKey(x => x.EventSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.SaleStartDate)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.SaleEndDate)
                .HasColumnType("timestamp with time zone");

        });
    }

    private static void ConfigureCheckIn(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.ToTable("CheckIn");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Notes).HasMaxLength(1000);

            entity.HasIndex(x => new { x.TenantId, x.TicketId }).IsUnique();

            entity.HasOne(x => x.Ticket)
                .WithOne(x => x.CheckIn)
                .HasForeignKey<CheckIn>(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CheckedInByUser)
                .WithMany(x => x.CheckIns)
                .HasForeignKey(x => x.CheckedInByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // ================= BOOKINGS =================

    private static void ConfigureBooking(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Booking", t =>
            {
                t.HasCheckConstraint("CK_Booking_TotalAmount", "\"TotalAmount\" >= 0");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ReferenceNumber).IsRequired().HasMaxLength(100);
            entity.Property(x => x.TotalAmount).HasPrecision(18, 2);

            entity.HasIndex(x => new { x.TenantId, x.ReferenceNumber }).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

        });
    }

    private static void ConfigureBookingItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.ToTable("BookingItem", t =>
            {
                t.HasCheckConstraint("CK_BookingItem_Quantity", "\"Quantity\" > 0");
                t.HasCheckConstraint("CK_BookingItem_UnitPrice", "\"UnitPrice\" >= 0");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UnitPrice).HasPrecision(18, 2);

            entity.HasOne(x => x.Booking)
                .WithMany(x => x.BookingItems)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.TicketType)
                .WithMany(x => x.BookingItems)
                .HasForeignKey(x => x.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EventSection)
                .WithMany(x => x.BookingItems)
                .HasForeignKey(x => x.EventSectionId)
                .OnDelete(DeleteBehavior.Restrict);

        });
    }


    // ================= PAYMENTS =================

    private static void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment", t =>
            {
                t.HasCheckConstraint("CK_Payment_Amount", "\"Amount\" > 0");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.TransactionId).HasMaxLength(200);

            entity.HasIndex(x => new { x.TenantId, x.TransactionId })
                   .IsUnique()
                   .HasFilter("\"TransactionId\" IS NOT NULL");

            entity.HasOne(x => x.Booking)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.PaymentMethod)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

        });
    }

    private static void ConfigurePaymentMethod(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("PaymentMethod");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(500);

            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });
    }

    // ================= REVIEW =================

    private static void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review", t =>
            {
                t.HasCheckConstraint("CK_Review_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Comment).HasMaxLength(2000);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.UserId }).IsUnique();

            entity.HasOne(x => x.Event)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        });
    }

    // ================= COUPONS =================

    private static void ConfigureDiscountCoupon(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountCoupon>(entity =>
        {
            entity.ToTable("DiscountCoupon", t =>
            {
                t.HasCheckConstraint("CK_DiscountCoupon_DiscountValue", "\"DiscountValue\" > 0");
                t.HasCheckConstraint("CK_DiscountCoupon_UsageLimit", "\"UsageLimit\" IS NULL OR \"UsageLimit\" > 0");
                t.HasCheckConstraint("CK_DiscountCoupon_UsageCount", "\"UsageCount\" >= 0");
                t.HasCheckConstraint("CK_DiscountCoupon_ValidRange", "\"ValidTo\" > \"ValidFrom\"");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Code).IsRequired().HasMaxLength(100);
            entity.Property(x => x.DiscountValue).HasPrecision(18, 2);

            entity.HasIndex(x => new { x.TenantId, x.EventId, x.Code }).IsUnique();

            entity.HasOne(x => x.Event)
                .WithMany(x => x.DiscountCoupons)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

        });
    }

    // ================= NOTIFICATIONS =================
    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Message).IsRequired().HasMaxLength(2000);

            entity.HasIndex(x => new { x.TenantId, x.UserId, x.IsRead });

            entity.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }


    // ================= AI =================

    private static void ConfigureAIRequestLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AIRequestLog>(entity =>
        {
            entity.ToTable("AIRequestLog", t =>
            {
                t.HasCheckConstraint("CK_AIRequestLog_TokensUsed", "\"TokensUsed\" >= 0");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Prompt).IsRequired().HasMaxLength(4000);
            entity.Property(x => x.ResponseSummary).HasMaxLength(4000);

            entity.HasOne(x => x.User)
                .WithMany(x => x.AIRequestLogs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        });
    }

}
