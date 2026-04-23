using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eventix.Domain.Entities;

namespace Eventix.Infrastructure.Persistence.Database
{
    public class PublicDbContext : DbContext
    {
        public PublicDbContext(DbContextOptions<PublicDbContext> options) : base(options) { }
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Speaker> Speakers => Set<Speaker>();
        public DbSet<EventSession> EventSessions => Set<EventSession>();
        public DbSet<CheckIn> CheckIns => Set<CheckIn>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Review> Reviews => Set<Review>();

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
