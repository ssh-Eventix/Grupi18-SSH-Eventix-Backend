namespace Eventix.Infrastructure.Auth;

public enum Permission
{
    // Events
    EventsCreate,
    EventsRead,
    EventsUpdate,
    EventsDelete,

    // Tickets
    TicketsCreate,
    TicketsRead,
    TicketsUpdate,
    TicketsDelete,
    TicketsPurchase,

    // Venues
    VenuesCreate,
    VenuesRead,
    VenuesUpdate,
    VenuesDelete,

    // Users (tenant-scoped user management)
    UsersCreate,
    UsersRead,
    UsersUpdate,
    UsersDelete,
    UsersAssignRoles
}

