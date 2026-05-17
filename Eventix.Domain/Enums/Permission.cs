namespace Eventix.Domain.Enums;

public enum Permission
{
    // SuperAdmin / platform
    ManageTenants,
    ViewTenants,
    CreateTenants,
    UpdateTenants,
    DeleteTenants,
    ImpersonateTenant,

    // Users & roles
    ManageUsers,
    ViewUsers,
    CreateUsers,
    UpdateUsers,
    DeleteUsers,
    ManageRoles,
    AssignRoles,

    // Events
    ManageEvents,
    ViewEvents,
    CreateEvents,
    UpdateEvents,
    DeleteEvents,
    PublishEvents,

    // Venues / sections
    ManageVenues,
    ViewVenues,
    CreateVenues,
    UpdateVenues,
    DeleteVenues,
    ManageVenueSections,

    // Event sections / ticket types
    ManageEventSections,
    ManageTicketTypes,
    CreateTicketTypes,
    UpdateTicketTypes,
    DeleteTicketTypes,

    // Bookings / orders
    ManageBookings,
    ViewBookings,
    CreateBookings,
    CancelBookings,
    RefundBookings,

    // Tickets
    ViewTickets,
    BuyTickets,
    ScanTickets,
    CheckInTickets,
    ValidateTickets,
    CancelTickets,

    // Payments
    ManagePayments,
    ViewPayments,
    RefundPayments,

    // Reviews
    ManageReviews,
    ViewReviews,
    DeleteReviews,

    // Reports / dashboard
    ViewReports,
    ViewDashboard,
    ExportReports,

    // Notifications
    ManageNotifications,
    ViewNotifications,

    // AI module
    UseAI,
    ViewAIRequestLogs,

    // Archive / audit
    ViewAuditLogs,
    ViewArchiveRecords,
    RestoreArchiveRecords
}