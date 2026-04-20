using System;
using System.Threading.Tasks;

namespace Eventix.Application.Interfaces.Services;

public interface ITenantService
{
    Task<Guid> CreateTenantAsync(string name, string slug);
}