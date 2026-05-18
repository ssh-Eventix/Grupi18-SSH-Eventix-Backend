using Eventix.Application.Interfaces.Common;
using Eventix.Domain.Entities;
using Eventix.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Eventix.Infrastructure.Services;

public class PublicSuperAdminSeeder
{
    private readonly PublicDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public PublicSuperAdminSeeder(
        PublicDbContext db,
        IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var role = await _db.PublicRoles
            .FirstOrDefaultAsync(x => x.NormalizedName == "SUPERADMIN", ct);

        if (role is null)
        {
            role = new PublicRole
            {
                Id = Guid.NewGuid(),
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                Description = "Platform super admin",
                CreatedAtUtc = now
            };

            _db.PublicRoles.Add(role);
            await _db.SaveChangesAsync(ct);
        }

        var email = "superadmin@eventix.test";

        var user = await _db.PublicUsers
            .FirstOrDefaultAsync(x => x.Email == email, ct);

        if (user is null)
        {
            user = new PublicUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = "Super Admin",
                PasswordHash = _passwordHasher.Hash("Admin123!"),
                IsActive = true,
                CreatedAtUtc = now
            };

            _db.PublicUsers.Add(user);
        }
        else
        {
            user.FullName = "Super Admin";
            user.PasswordHash = _passwordHasher.Hash("Admin123!");
            user.IsActive = true;
        }

        await _db.SaveChangesAsync(ct);

        var hasRole = await _db.PublicUserRoles
            .AnyAsync(x =>
                x.PublicUserId == user.Id &&
                x.PublicRoleId == role.Id,
                ct);

        if (!hasRole)
        {
            _db.PublicUserRoles.Add(new PublicUserRole
            {
                PublicUserId = user.Id,
                PublicRoleId = role.Id
            });

            await _db.SaveChangesAsync(ct);
        }
    }
}