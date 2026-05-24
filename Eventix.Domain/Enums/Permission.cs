namespace Eventix.Domain.Enums;

public enum Permission
{
    // SuperAdmin / platform
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
    ViewRoles,

    // Events
    SearchEvents,
    ViewEvents,
    CreateEvents,
    UpdateEvents,
    DeleteEvents,

    // Venues / sections
    ManageVenues,
    ViewVenues,
    CreateVenues,
    UpdateVenues,
    DeleteVenues,
    ManageVenueSections,

    // Event sections / ticket types
    ManageEventSections,
    ViewEventSections,
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
    ViewReviews,
    DeleteReviews,
    CreateReviews,
    UpdateReviews,

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
    ArchiveRecords,

    //Event session, categories and speakers
    ViewEventCategories,
    CreateEventCategories,
    UpdateEventCategories,
    DeleteEventCategories,

    ManageEventSessions,

    CreateSpeakers,
    UpdateSpeakers,
    DeleteSpeakers,

    //Discount coupons
    ManageDiscountCoupons,
    ViewDiscountCoupons,
    CreateDiscountCoupons,
    UpdateDiscountCoupons,
    DeleteDiscountCoupons,

    //Check ins
    ViewCheckIns,
    ManageCheckIns,

    // Payment methods
    ManagePaymentMethods,
    ViewPaymentMethods,

    // Tenant email domains
    ManageTenantEmailDomains,
    ViewTenantEmailDomains,

    // Public users / public roles
    ManagePublicUsers,
    ViewPublicUsers,
    ManagePublicRoles,
    ViewPublicRoles,

    // Refresh tokens / security
    ViewRefreshTokens,
    RevokeRefreshTokens,

    ViewEventSessions,
    ViewSpeakers,
    ViewTicketTypes,
    ViewVenueSections
}