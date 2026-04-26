namespace Eventix.Application.Interfaces.Common;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}

