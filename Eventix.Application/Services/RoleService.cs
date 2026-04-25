using Eventix.Application.DTOs.Roles;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<List<RoleResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return roles.Where(r => !r.IsDeleted).Select(Map).ToList();
    }

    public async Task<RoleResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        return role is null || role.IsDeleted ? null : Map(role);
    }

    public async Task<RoleResponseDTO> CreateAsync(CreateRoleDTO dto, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _roleRepository.AddAsync(entity, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRoleDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(entity);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(entity);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static RoleResponseDTO Map(Role r) => new()
    {
        Id = r.Id,
        TenantId = r.TenantId,
        Name = r.Name,
        Description = r.Description,
        CreatedAtUtc = r.CreatedAtUtc,
        UpdatedAtUtc = r.UpdatedAtUtc
    };
}

