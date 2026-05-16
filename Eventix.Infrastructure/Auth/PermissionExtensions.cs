using Eventix.Domain.Enums;

namespace Eventix.Application.Authorization;

public static class PermissionExtensions
{
    public static string ToPolicyName(this Permission permission)
    {
        return $"Permission:{permission}";
    }
}