namespace Eventix.Infrastructure.Auth;

public static class PermissionExtensions
{
    public static bool IsTenantScoped(this Permission permission)
    {
        switch (permission)
        {
            case Permission.EventsCreate:
            case Permission.EventsRead:
            case Permission.EventsUpdate:
            case Permission.EventsDelete:
            case Permission.TicketsCreate:
            case Permission.TicketsRead:
            case Permission.TicketsUpdate:
            case Permission.TicketsDelete:
            case Permission.TicketsPurchase:
            case Permission.VenuesCreate:
            case Permission.VenuesRead:
            case Permission.VenuesUpdate:
            case Permission.VenuesDelete:
            case Permission.UsersCreate:
            case Permission.UsersRead:
            case Permission.UsersUpdate:
            case Permission.UsersDelete:
            case Permission.UsersAssignRoles:
                return true;
            default:
                return true;
        }
    }
    
    public static bool IsAdminGrantedByDefault(this Permission permission)
    {
        switch (permission)
        {
            case Permission.EventsCreate:
            case Permission.EventsRead:
            case Permission.EventsUpdate:
            case Permission.EventsDelete:
            case Permission.TicketsCreate:
            case Permission.TicketsRead:
            case Permission.TicketsUpdate:
            case Permission.TicketsDelete:
            case Permission.TicketsPurchase:
            case Permission.VenuesCreate:
            case Permission.VenuesRead:
            case Permission.VenuesUpdate:
            case Permission.VenuesDelete:
            case Permission.UsersCreate:
            case Permission.UsersRead:
            case Permission.UsersUpdate:
            case Permission.UsersDelete:
            case Permission.UsersAssignRoles:
                return true;
            default:
                return false;
        }
    }
}

