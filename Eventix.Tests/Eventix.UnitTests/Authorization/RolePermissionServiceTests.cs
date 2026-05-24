using Eventix.Domain.Enums;
using Eventix.Infrastructure.Services;

namespace Eventix.UnitTests.Authorization;

public class RolePermissionServiceTests
{
    private readonly RolePermissionService _service = new();

    [Fact]
    public void SuperAdmin_Should_Have_All_Permissions()
    {
        foreach (var permission in Enum.GetValues<Permission>())
        {
            Assert.True(_service.RoleHasPermission("SuperAdmin", permission));
        }
    }

    [Fact]
    public void Buyer_Should_Be_Able_To_Buy_Tickets()
    {
        Assert.True(_service.RoleHasPermission("Buyer", Permission.BuyTickets));
    }

    [Fact]
    public void Buyer_Should_Not_Manage_Users()
    {
        Assert.False(_service.RoleHasPermission("Buyer", Permission.ManageUsers));
    }

    [Fact]
    public void Staff_Should_Scan_Tickets()
    {
        Assert.True(_service.RoleHasPermission("Staff", Permission.ScanTickets));
    }

    [Fact]
    public void Unknown_Role_Should_Not_Have_Permission()
    {
        Assert.False(_service.RoleHasPermission("Unknown", Permission.ViewEvents));
    }
}