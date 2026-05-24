using Eventix.Api.Controllers;
using Eventix.Application.DTOs.TenantEmailDomains;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.ApiTests.TenantEmailDomains;

public class TenantEmailDomainControllerApiTests
{
    [Fact]
    public async Task GetById_When_Domain_Exists_Should_Return_Ok()
    {
        var service = new FakeTenantEmailDomainService();
        var controller = new TenantEmailDomainsController(service);

        var created = await service.CreateAsync(
            new CreateTenantEmailDomainDTO
            {
                TenantId = Guid.NewGuid(),
                Domain = "eventix.com",
                DefaultRoleName = "Buyer",
                AutoApprove = true
            },
            CancellationToken.None);

        var response = await controller.GetById(created.Id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(response);
        var value = Assert.IsType<TenantEmailDomainResponseDTO>(ok.Value);

        Assert.Equal("eventix.com", value.Domain);
    }

    [Fact]
    public async Task GetById_When_Domain_Does_Not_Exist_Should_Return_NotFound()
    {
        var controller = new TenantEmailDomainsController(
            new FakeTenantEmailDomainService());

        var response = await controller.GetById(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async Task Create_Should_Return_CreatedAtAction()
    {
        var controller = new TenantEmailDomainsController(
            new FakeTenantEmailDomainService());

        var response = await controller.Create(
            new CreateTenantEmailDomainDTO
            {
                TenantId = Guid.NewGuid(),
                Domain = "company.com",
                DefaultRoleName = "Staff",
                AutoApprove = false
            },
            CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(response);
        var value = Assert.IsType<TenantEmailDomainResponseDTO>(created.Value);

        Assert.Equal(nameof(TenantEmailDomainsController.GetById), created.ActionName);
        Assert.Equal("company.com", value.Domain);
        Assert.Equal("Staff", value.DefaultRoleName);
        Assert.False(value.AutoApprove);
    }

    [Fact]
    public async Task Delete_When_Domain_Exists_Should_Return_NoContent()
    {
        var service = new FakeTenantEmailDomainService();
        var controller = new TenantEmailDomainsController(service);

        var created = await service.CreateAsync(
            new CreateTenantEmailDomainDTO
            {
                TenantId = Guid.NewGuid(),
                Domain = "delete.com"
            },
            CancellationToken.None);

        var response = await controller.Delete(created.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task Delete_When_Domain_Does_Not_Exist_Should_Return_NotFound()
    {
        var controller = new TenantEmailDomainsController(
            new FakeTenantEmailDomainService());

        var response = await controller.Delete(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.IsType<NotFoundResult>(response);
    }

    private class FakeTenantEmailDomainService : ITenantEmailDomainService
    {
        private readonly List<TenantEmailDomainResponseDTO> _items = new();

        public Task<List<TenantEmailDomainResponseDTO>> GetAllAsync(CancellationToken ct)
        {
            return Task.FromResult(_items);
        }

        public Task<List<TenantEmailDomainResponseDTO>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct)
        {
            return Task.FromResult(
                _items.Where(x => x.TenantId == tenantId).ToList());
        }

        public Task<TenantEmailDomainResponseDTO?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return Task.FromResult(
                _items.FirstOrDefault(x => x.Id == id));
        }

        public Task<TenantEmailDomainResponseDTO> CreateAsync(CreateTenantEmailDomainDTO dto, CancellationToken ct)
        {
            var item = new TenantEmailDomainResponseDTO
            {
                Id = Guid.NewGuid(),
                TenantId = dto.TenantId,
                Domain = dto.Domain.Trim().ToLower(),
                DefaultRoleName = dto.DefaultRoleName,
                AutoApprove = dto.AutoApprove
            };

            _items.Add(item);

            return Task.FromResult(item);
        }

        public Task<TenantEmailDomainResponseDTO?> UpdateAsync(Guid id, UpdateTenantEmailDomainDTO dto, CancellationToken ct)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);

            if (item is null)
                return Task.FromResult<TenantEmailDomainResponseDTO?>(null);

            item.Domain = dto.Domain.Trim().ToLower();
            item.DefaultRoleName = dto.DefaultRoleName;
            item.AutoApprove = dto.AutoApprove;

            return Task.FromResult<TenantEmailDomainResponseDTO?>(item);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);

            if (item is null)
                return Task.FromResult(false);

            _items.Remove(item);

            return Task.FromResult(true);
        }
    }
}