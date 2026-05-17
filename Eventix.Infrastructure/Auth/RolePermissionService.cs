using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Enums;

namespace Eventix.Infrastructure.Services;

public class RolePermissionService : IRolePermissionService
{
    private static readonly Dictionary<string, Permission[]> PermissionsByRole =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["SuperAdmin"] =
            [
                Permission.ManageTenants,
                Permission.ViewTenants,
                Permission.CreateTenants,
                Permission.UpdateTenants,
                Permission.DeleteTenants,
                Permission.ImpersonateTenant,
                Permission.ViewAuditLogs,
                Permission.ViewArchiveRecords,
                Permission.RestoreArchiveRecords
            ],

            ["Admin"] =
            [
                Permission.ManageUsers,
                Permission.ViewUsers,
                Permission.CreateUsers,
                Permission.UpdateUsers,
                Permission.DeleteUsers,
                Permission.ManageRoles,
                Permission.AssignRoles,

                Permission.ManageEvents,
                Permission.ViewEvents,
                Permission.CreateEvents,
                Permission.UpdateEvents,
                Permission.DeleteEvents,
                Permission.PublishEvents,

                Permission.ManageVenues,
                Permission.ViewVenues,
                Permission.CreateVenues,
                Permission.UpdateVenues,
                Permission.DeleteVenues,
                Permission.ManageVenueSections,

                Permission.ManageEventSections,
                Permission.ManageTicketTypes,

                Permission.ManageBookings,
                Permission.ViewBookings,
                Permission.CancelBookings,
                Permission.RefundBookings,

                Permission.ViewTickets,
                Permission.ScanTickets,
                Permission.CheckInTickets,
                Permission.ValidateTickets,

                Permission.ManagePayments,
                Permission.ViewPayments,
                Permission.RefundPayments,

                Permission.ViewReports,
                Permission.ViewDashboard,
                Permission.ExportReports,

                Permission.ManageNotifications,
                Permission.ViewNotifications,

                Permission.UseAI,
                Permission.ViewAIRequestLogs
            ],

            ["Staff"] =
            [
                Permission.ViewEvents,
                Permission.ViewBookings,
                Permission.ViewTickets,
                Permission.ScanTickets,
                Permission.CheckInTickets,
                Permission.ValidateTickets,
                Permission.ViewDashboard,
                Permission.ViewNotifications
            ],

            ["Buyer"] =
            [
                Permission.ViewEvents,
                Permission.CreateBookings,
                Permission.CancelBookings,
                Permission.BuyTickets,
                Permission.ViewTickets,
                Permission.ViewNotifications,
                Permission.UseAI
            ]
        };

    public bool RoleHasPermission(string role, Permission permission)
    {
        return PermissionsByRole.TryGetValue(role, out var permissions)
            && permissions.Contains(permission);
    }

    public bool UserHasPermission(IEnumerable<string> roles, Permission permission)
    {
        return roles.Any(role => RoleHasPermission(role, permission));
    }
}