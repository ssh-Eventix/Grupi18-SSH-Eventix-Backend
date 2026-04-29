using Eventix.Application.DTOs.UserRoles;
using Eventix.Application.Interfaces.Common;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;


    public UserRoleService(IUserRoleRepository userRoleRepository, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<List<UserRoleResponseDTO>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || user.IsDeleted) return new List<UserRoleResponseDTO>();

        var roles = await _userRoleRepository.GetByUserIdAsync(userId, cancellationToken);
        return roles.Where(ur => !ur.IsDeleted).Select(Map).ToList();
    }

    public async Task<UserRoleResponseDTO> AssignAsync(CreateUserRoleDTO dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user is null || user.IsDeleted)
            throw new InvalidOperationException("Invalid user");

        var role = await _roleRepository.GetByIdAsync(dto.RoleId, cancellationToken);
        if (role is null || role.IsDeleted)
            throw new InvalidOperationException("Invalid role");

        if (await _userRoleRepository.ExistsAsync(dto.UserId, dto.RoleId, cancellationToken))
            throw new InvalidOperationException("User already has this role assigned");

        var entity = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            RoleId = dto.RoleId,
            AssignedAt = DateTime.UtcNow
        };

        await _userRoleRepository.AddAsync(entity, cancellationToken);
        await _userRoleRepository.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userRoleRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        await _userRoleRepository.DeleteAsync(entity);
        await _userRoleRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static UserRoleResponseDTO Map(UserRole ur) => new()
    {
        Id = ur.Id,
        UserId = ur.UserId,
        RoleId = ur.RoleId,
        AssignedAt = ur.AssignedAt
    };
}

