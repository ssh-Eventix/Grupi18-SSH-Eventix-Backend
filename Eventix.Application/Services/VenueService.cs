using Eventix.Application.DTOs.Venues;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _repository;
    private readonly ITenantContext _tenantContext;

    public VenueService(IVenueRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<VenueResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var venues = await _repository.GetAllAsync(cancellationToken);
        return venues.Select(Map);
    }

    public async Task<VenueResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var venue = await _repository.GetByIdAsync(id, cancellationToken);
        return venue is null ? null : Map(venue);
    }

    public async Task<VenueResponseDTO> CreateAsync(CreateVenueDTO dto, CancellationToken cancellationToken = default)
    {
        var exists = await _repository.ExistsByCodeAsync(dto.Code, null, cancellationToken);
        if (exists)
            throw new Exception("Venue with same code already exists.");

        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,

            Name = dto.Name,
            Code = dto.Code,
            AddressLine1 = dto.AddressLine1,
            City = dto.City,
            Country = dto.Country,
            TotalCapacity = dto.TotalCapacity,

            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(venue, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(venue);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateVenueDTO dto, CancellationToken cancellationToken = default)
    {
        var venue = await _repository.GetByIdAsync(id, cancellationToken);
        if (venue is null) return false;

        var exists = await _repository.ExistsByCodeAsync(dto.Code, id, cancellationToken);
        if (exists)
            throw new Exception("Venue with same code already exists.");

        venue.Name = dto.Name;
        venue.Code = dto.Code;
        venue.AddressLine1 = dto.AddressLine1;
        venue.City = dto.City;
        venue.Country = dto.Country;
        venue.TotalCapacity = dto.TotalCapacity;

        venue.IsIndoor = dto.IsIndoor;
        venue.IsAccessible = dto.IsAccessible;

        venue.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(venue);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var venue = await _repository.GetByIdAsync(id, cancellationToken);
        if (venue is null) return false;

        venue.IsDeleted = true;
        venue.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(venue);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static VenueResponseDTO Map(Venue v) => new()
    {
        Id = v.Id,
        TenantId = v.TenantId,

        Name = v.Name,
        Code = v.Code,

        AddressLine1 = v.AddressLine1,
        City = v.City,
        Country = v.Country,

        TotalCapacity = v.TotalCapacity,

        IsIndoor = v.IsIndoor,
        IsAccessible = v.IsAccessible,

        CreatedAtUtc = v.CreatedAtUtc
    };
}