namespace Eventix.Application.Interfaces.Common;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }
}