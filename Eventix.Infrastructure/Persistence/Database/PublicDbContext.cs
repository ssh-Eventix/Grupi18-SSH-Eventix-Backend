using Eventix.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Persistence.Database;

public class PublicDbContext : DbContext
{
    public PublicDbContext(DbContextOptions<PublicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<PublicUser> PublicUsers => Set<PublicUser>();
    public DbSet<PublicRole> PublicRoles => Set<PublicRole>();
    public DbSet<PublicUserRole> PublicUserRoles => Set<PublicUserRole>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TenantImpersonationLog> TenantImpersonationLogs => Set<TenantImpersonationLog>();
    public DbSet<ArchiveRecord> ArchiveRecords => Set<ArchiveRecord>();
    public DbSet<TenantEmailDomain> TenantEmailDomains => Set<TenantEmailDomain>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueSection> VenueSections => Set<VenueSection>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        ConfigureTenant(modelBuilder);
        ConfigurePublicRole(modelBuilder);
        ConfigurePublicUserRole(modelBuilder);
        ConfigurePublicUser(modelBuilder);
        ConfigureTenantImpersonationLog(modelBuilder);
        ConfigureArchiveRecord(modelBuilder);
        ConfigureRefreshToken(modelBuilder);
        ConfigureTenantEmailDomain(modelBuilder);
        ConfigurePasswordResetToken(modelBuilder);
        ConfigureVenue(modelBuilder);
        ConfigureVenueSection(modelBuilder);
        ConfigurePaymentMethod(modelBuilder);
        ConfigureAuditLog(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigurePasswordResetToken(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.ToTable("PasswordResetTokens");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.TokenHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.ExpiresAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(x => x.UsedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => new { x.PublicUserId, x.TenantId, x.IsDeleted });

            entity.HasOne(x => x.PublicUser)
                .WithMany(x => x.PasswordResetTokens)
                .HasForeignKey(x => x.PublicUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePublicRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicRole>(entity =>
        {
            entity.ToTable("PublicRoles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.NormalizedName)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => x.NormalizedName)
                .IsUnique();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone");
        });
    }

    private static void ConfigureArchiveRecord(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchiveRecord>(entity =>
        {
            entity.ToTable("ArchiveRecords", "public");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.SchemaName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.EntityName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DataJson)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(x => x.ArchivedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.SchemaName);
            entity.HasIndex(x => new { x.TenantId, x.EntityName, x.EntityId });
            entity.HasIndex(x => x.ArchiveYear);
        });
    }

    private static void ConfigurePublicUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicUser>(entity =>
        {
            entity.ToTable("PublicUsers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.FullName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.LastLoginAtUtc)
                .HasColumnType("timestamp with time zone");
        });
    }

    private static void ConfigurePublicUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicUserRole>(entity =>
        {
            entity.ToTable("PublicUserRoles");

            entity.HasKey(x => new
            {
                x.PublicUserId,
                x.PublicRoleId
            });

            entity.HasOne(x => x.PublicUser)
                .WithMany(x => x.PublicUserRoles)
                .HasForeignKey(x => x.PublicUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.PublicRole)
                .WithMany(x => x.PublicUserRoles)
                .HasForeignKey(x => x.PublicRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureRefreshToken(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TokenHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasIndex(x => x.TokenHash)
                .IsUnique();

            entity.Property(x => x.ExpiresAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.RevokedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.ReplacedByTokenHash)
                .HasMaxLength(500);

            entity.HasOne(x => x.PublicUser)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.PublicUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTenantImpersonationLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantImpersonationLog>(entity =>
        {
            entity.ToTable("TenantImpersonationLogs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.StartedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(x => x.ExpiresAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(x => x.RevokedAtUtc)
                .HasColumnType("timestamp with time zone");

            entity.Property(x => x.Reason)
                .HasMaxLength(1000);

            entity.Property(x => x.Event)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(x => x.SuperAdminUser)
                .WithMany()
                .HasForeignKey(x => x.SuperAdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.TargetUser)
                .WithMany()
                .HasForeignKey(x => x.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.TargetTenant)
                .WithMany()
                .HasForeignKey(x => x.TargetTenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.SuperAdminUserId);
            entity.HasIndex(x => x.TargetTenantId);
            entity.HasIndex(x => new { x.TargetTenantId, x.IsActive });
        });
    }

    private static void ConfigureTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();
                
            entity.Property(x => x.Slug)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.SchemaName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.ContactEmail)
                .HasMaxLength(200);

            entity.Property(x => x.City)
                .HasMaxLength(100);

            entity.Property(x => x.Country)
                .HasMaxLength(100);

            entity.Property(x => x.LogoUrl)
                .HasMaxLength(500);

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.Property(x => x.IsTrial)
                .HasDefaultValue(false);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasIndex(x => x.Slug)
                .IsUnique();

            entity.HasIndex(x => x.SchemaName)
                .IsUnique();
        });
    }

    private static void ConfigureTenantEmailDomain(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantEmailDomain>(entity =>
        {
            entity.ToTable("TenantEmailDomains");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Domain)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.DefaultRoleName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.AutoApprove)
                .HasDefaultValue(false);

            entity.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.TenantId, x.Domain })
                .IsUnique();
        });
    }

    private static void ConfigureVenue(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("Venue", "public");

            entity.HasKey(x => x.Id);
        });
    }

    private static void ConfigureVenueSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VenueSection>(entity =>
        {
            entity.ToTable("VenueSection", "public");

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Venue)
                .WithMany(x => x.Sections)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePaymentMethod(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("PaymentMethod", "public");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
    }

    private static void ConfigureAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLog", "public");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TenantName).HasMaxLength(200);
            entity.Property(x => x.UserEmail).HasMaxLength(250);

            entity.Property(x => x.EntityName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.OldValues).HasColumnType("jsonb");
            entity.Property(x => x.NewValues).HasColumnType("jsonb");

            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => new { x.EntityName, x.EntityId });
            entity.HasIndex(x => x.CreatedAtUtc);
        });
    }
}
