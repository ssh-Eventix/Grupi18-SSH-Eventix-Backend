using Eventix.Domain.Entities;
using Eventix.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Database;

public class TenantDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public string CurrentSchema => _tenantContext.Schema;

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueSection> VenueSections => Set<VenueSection>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventSection> EventSections => Set<EventSection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(CurrentSchema);

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
        });

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

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.AddressLine1)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.AddressLine2)
                .HasMaxLength(200);

            entity.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.State)
                .HasMaxLength(100);

            entity.Property(x => x.PostalCode)
                .HasMaxLength(20);

            entity.Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Latitude)
                .HasColumnType("numeric(9,6)");

            entity.Property(x => x.Longitude)
                .HasColumnType("numeric(9,6)");

            entity.Property(x => x.ContactEmail)
                .HasMaxLength(200);

            entity.Property(x => x.ContactPhone)
                .HasMaxLength(50);

            entity.Property(x => x.SeatingMapImageUrl)
                .HasMaxLength(500);
        });

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

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.DefaultBasePrice)
                .HasColumnType("numeric(18,2)");

            entity.HasOne(x => x.Venue)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Cascade);
        });

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

            entity.Property(x => x.Subtitle)
                .HasMaxLength(300);

            entity.Property(x => x.Description)
                .HasMaxLength(4000);

            entity.Property(x => x.ShortDescription)
                .HasMaxLength(1000);

            entity.Property(x => x.OrganizerName)
                .HasMaxLength(150);

            entity.Property(x => x.OrganizerEmail)
                .HasMaxLength(200);

            entity.Property(x => x.OrganizerPhone)
                .HasMaxLength(50);

            entity.Property(x => x.BannerImageUrl)
                .HasMaxLength(500);

            entity.Property(x => x.ThumbnailImageUrl)
                .HasMaxLength(500);

            entity.Property(x => x.Tags)
                .HasMaxLength(1000);

            entity.Property(x => x.TermsAndConditions)
                .HasMaxLength(4000);

            entity.Property(x => x.RefundPolicy)
                .HasMaxLength(4000);

            entity.Property(x => x.MinPrice)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.MaxPrice)
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            entity.HasOne(x => x.Venue)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.EventCategory)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

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

            entity.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(x => x.Benefits)
                .HasMaxLength(2000);

            entity.Property(x => x.Notes)
                .HasMaxLength(2000);

            entity.HasOne(x => x.Event)
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.VenueSection)
                .WithMany(x => x.EventSections)
                .HasForeignKey(x => x.VenueSectionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}