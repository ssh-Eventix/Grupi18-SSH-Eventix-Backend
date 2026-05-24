using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Enums;

namespace Eventix.Infrastructure.Services;

public class RolePermissionService : IRolePermissionService
{
    private static readonly Dictionary<string, Permission[]> PermissionsByRole =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["SuperAdmin"] = Enum.GetValues<Permission>(),

            ["Admin"] =
            [
                Permission.ManageUsers,
                Permission.ViewUsers,
                Permission.CreateUsers,
                Permission.UpdateUsers,
                Permission.DeleteUsers,
                Permission.ManageRoles,
                Permission.AssignRoles,

                Permission.SearchEvents,
                Permission.ViewEvents,
                Permission.CreateEvents,
                Permission.UpdateEvents,
                Permission.DeleteEvents,

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
                Permission.ViewAIRequestLogs,

                Permission.CreateTicketTypes,
                Permission.UpdateTicketTypes,
                Permission.DeleteTicketTypes,
                Permission.ViewAuditLogs,

                Permission.ViewEventCategories,
                Permission.CreateEventCategories,
                Permission.UpdateEventCategories,
                Permission.DeleteEventCategories,

                Permission.ManageEventSessions,

                Permission.CreateSpeakers,
                Permission.UpdateSpeakers,
                Permission.DeleteSpeakers,

                Permission.ManageDiscountCoupons,
                Permission.ViewDiscountCoupons,
                Permission.CreateDiscountCoupons,
                Permission.UpdateDiscountCoupons,
                Permission.DeleteDiscountCoupons,

                Permission.ViewCheckIns,
                Permission.ManageCheckIns,

                Permission.ManagePaymentMethods,
                Permission.ViewPaymentMethods,

                Permission.ManageTenantEmailDomains,
                Permission.ViewTenantEmailDomains,

                Permission.ViewReviews,
                Permission.CreateReviews,
                Permission.UpdateReviews,
                Permission.DeleteReviews,

                Permission.ViewEventSections,
                Permission.ViewEventSessions,
                Permission.ViewRoles,
                Permission.ViewSpeakers,
                Permission.ViewTicketTypes,
                Permission.ViewVenueSections
            ],

            ["Staff"] =
            [
                Permission.ViewEvents,
                Permission.SearchEvents,
                Permission.ViewBookings,
                Permission.ViewTickets,
                Permission.ScanTickets,
                Permission.CheckInTickets,
                Permission.ValidateTickets,
                Permission.ViewDashboard,
                Permission.ViewNotifications,
                Permission.CancelTickets,
                Permission.ViewCheckIns,
                Permission.ManageCheckIns,
                Permission.ViewEventSessions,
                Permission.ViewSpeakers,
                Permission.ViewEventSections,
                Permission.ViewTicketTypes
            ],

            ["Buyer"] =
            [
                Permission.ViewEvents,
                Permission.SearchEvents,
                Permission.CreateBookings,
                Permission.ViewBookings,
                Permission.CancelBookings,
                Permission.BuyTickets,
                Permission.ViewTickets,
                Permission.ViewNotifications,
                Permission.UseAI,
                Permission.CreateReviews,
                Permission.UpdateReviews,
                Permission.ViewDiscountCoupons,
                Permission.ViewSpeakers,
                Permission.ViewEventSessions,
                Permission.ViewTicketTypes
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