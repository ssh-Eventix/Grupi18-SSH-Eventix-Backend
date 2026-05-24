using Eventix.Infrastructure.Persistence.Database;
using Eventix.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Services;

public class PublicRoleSeeder
{
    private readonly PublicDbContext _db;

    public PublicRoleSeeder(PublicDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync()
    {
        var buyerExists = await _db.PublicRoles
            .AnyAsync(x => x.NormalizedName == "BUYER");

        if (!buyerExists)
        {
            _db.PublicRoles.Add(new PublicRole
            {
                Name = "Buyer",
                NormalizedName = "BUYER",
                Description = "Public buyer role"
            });
        }

        var superAdminExists = await _db.PublicRoles
            .AnyAsync(x => x.NormalizedName == "SUPERADMIN");

        if (!superAdminExists)
        {
            _db.PublicRoles.Add(new PublicRole
            {
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                Description = "Platform super admin"
            });
        }

        await _db.SaveChangesAsync();
    }
}