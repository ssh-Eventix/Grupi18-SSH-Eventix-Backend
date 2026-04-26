using Eventix.Application.DTOs.Users;
using Eventix.Application.Interfaces.Repositories;
using Eventix.Application.Interfaces.Services;
using Eventix.Domain.Entities;

namespace Eventix.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly Interfaces.Common.IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, Interfaces.Common.IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Where(u => !u.IsDeleted).Select(MapToDto).ToList();
    }

    public async Task<UserResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user is null || user.IsDeleted ? null : MapToDto(user);
    }

    public async Task<UserResponseDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user is null || user.IsDeleted ? null : MapToDto(user);
    }

    public async Task<UserResponseDTO> CreateAsync(CreateUserDTO dto, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existing is not null && !existing.IsDeleted)
            throw new InvalidOperationException("A user with this email already exists.");

        var entity = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
            IsActive = dto.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _userRepository.AddAsync(entity, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateUserDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            entity.PasswordHash = _passwordHasher.Hash(dto.Password);

        entity.IsActive = dto.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _userRepository.UpdateAsync(entity);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _userRepository.UpdateAsync(entity);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static UserResponseDTO MapToDto(User u) => new()
    {
        Id = u.Id,
        TenantId = u.TenantId,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        IsActive = u.IsActive,
        CreatedAtUtc = u.CreatedAtUtc,
        UpdatedAtUtc = u.UpdatedAtUtc
    };
}

