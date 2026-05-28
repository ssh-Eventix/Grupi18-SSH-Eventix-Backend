using System.Security.Claims;
using Eventix.Application.Interfaces.Common;

namespace Eventix.Api.Helpers;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value =
                _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
                ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("userId");

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
        ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("email");
}