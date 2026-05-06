
using Microsoft.AspNetCore.Authorization;

namespace Eventix.Infrastructure.Auth;

public class PermissionRequirement : IAuthorizationRequirement
{
	public Permission Permission { get; }

	public PermissionRequirement(Permission permission)
	{
		Permission = permission;
	}
}


