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
    public DbSet<TenantImpersonationLog> TenantImpersonationLogs => Set<TenantImpersonationLog>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        ConfigureTenant(modelBuilder);
        ConfigurePublicRole(modelBuilder);
        ConfigurePublicUserRole(modelBuilder);
        ConfigurePublicUser(modelBuilder);
        ConfigureTenantImpersonationLog(modelBuilder);

        base.OnModelCreating(modelBuilder);
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

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
    }

    private static void ConfigurePublicUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicUser>(entity =>
        {
            entity.ToTable("PublicUsers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
        });
    }

    private static void ConfigurePublicUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicUserRole>(entity =>
        {
            entity.ToTable("PublicUserRoles");
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.PublicUserId, x.PublicRoleId })
                .IsUnique();

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

    private static void ConfigureTenantImpersonationLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantImpersonationLog>(entity =>
        {
            entity.ToTable("TenantImpersonationLogs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).IsRequired();
            entity.Property(x => x.TargetTenantUserId).IsRequired();
            entity.Property(x => x.StartedAtUtc).IsRequired();
            entity.Property(x => x.ExpiresAtUtc).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.ImpersonatorPublicUserId);
            entity.HasIndex(x => x.ImpersonatorTenantUserId);
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
}