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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenants");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
                entity.Property(x => x.Slug).HasMaxLength(100).IsRequired();
                entity.Property(x => x.SchemaName).HasMaxLength(100).IsRequired();
                //entity.Property(x => x.Domain).HasMaxLength(200);

                entity.HasIndex(x => x.Slug).IsUnique();
                entity.HasIndex(x => x.SchemaName).IsUnique();
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Speaker>(entity =>
            {
                entity.ToTable("Speakers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.TenantId)
                    .IsRequired();

                entity.Property(x => x.FullName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Bio)
                    .HasMaxLength(1000);

                entity.Property(x => x.Email)
                    .HasMaxLength(150);

                entity.Property(x => x.Phone)
                    .HasMaxLength(50);

                entity.Property(x => x.ProfileImageUrl)
                    .HasMaxLength(500);

                entity.HasIndex(x => x.Email);
                entity.HasIndex(x => x.TenantId);
            });


        }
    }
}