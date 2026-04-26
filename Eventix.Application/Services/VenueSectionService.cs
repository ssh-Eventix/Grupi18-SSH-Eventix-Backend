using Eventix.Application.DTOs.VenueSections;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;
using Eventix.Domain.Enums;

public class VenueSectionService : IVenueSectionService
{
    private readonly IVenueSectionRepository _repository;
    private readonly ITenantContext _tenantContext;

    public VenueSectionService(
        IVenueSectionRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<VenueSectionResponseDTO>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(Map);
    }

    public async Task<VenueSectionResponseDTO?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : Map(entity);
    }

    public async Task<IEnumerable<VenueSectionResponseDTO>> GetByVenueIdAsync(Guid venueId)
    {
        var entities = await _repository.GetByVenueIdAsync(venueId);
        return entities.Select(Map);
    }

    public async Task<VenueSectionResponseDTO> CreateAsync(CreateVenueSectionDTO dto)
    {
        var entity = new VenueSection
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,
            VenueId = dto.VenueId,
            Name = dto.Name,
            Code = dto.Code,
            Capacity = dto.Capacity,
            SeatType = (SeatType)dto.SeatType,
            DefaultBasePrice = dto.DefaultBasePrice,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateVenueSectionDTO dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.Name = dto.Name;
        entity.Code = dto.Code;
        entity.Capacity = dto.Capacity;
        entity.SeatType = (SeatType)dto.SeatType;
        entity.IsActive = dto.IsActive;
        entity.DefaultBasePrice = dto.DefaultBasePrice;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static VenueSectionResponseDTO Map(VenueSection x)
    {
        return new VenueSectionResponseDTO
        {
            Id = x.Id,
            VenueId = x.VenueId,
            Name = x.Name,
            Code = x.Code,
            Capacity = x.Capacity,
            SeatType = (int)x.SeatType,
            IsActive = x.IsActive,
            DefaultBasePrice = x.DefaultBasePrice
        };
    }
}