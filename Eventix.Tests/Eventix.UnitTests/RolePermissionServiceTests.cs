using Eventix.Domain.Enums;
using Eventix.Infrastructure.Services;
using Xunit;

namespace Eventix.UnitTests;

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
    public void Admin_Should_Manage_Users()
    {
        Assert.True(_service.RoleHasPermission("Admin", Permission.ManageUsers));
    }

    [Fact]
    public void Staff_Should_Not_Create_Tenants()
    {
        Assert.False(_service.RoleHasPermission("Staff", Permission.CreateTenants));
    }

    [Fact]
    public void Buyer_Should_Buy_Tickets()
    {
        Assert.True(_service.RoleHasPermission("Buyer", Permission.BuyTickets));
    }

    [Fact]
    public void Buyer_Should_Not_Manage_Users()
    {
        Assert.False(_service.RoleHasPermission("Buyer", Permission.ManageUsers));
    }

    [Fact]
    public void User_With_Multiple_Roles_Should_Have_Permission_If_Any_Role_Has_It()
    {
        var roles = new[] { "Buyer", "Staff" };

        Assert.True(_service.UserHasPermission(roles, Permission.ScanTickets));
    }
}