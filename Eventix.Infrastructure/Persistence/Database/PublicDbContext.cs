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
                entity.Property(x => x.Domain).HasMaxLength(200);

                entity.HasIndex(x => x.Slug).IsUnique();
                entity.HasIndex(x => x.SchemaName).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
