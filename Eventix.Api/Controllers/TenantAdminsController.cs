using Eventix.Application.DTOs.TenantAdmins;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantAdminsController : ControllerBase
    {
        private readonly ITenantAdminService _service;

        public TenantAdminsController(ITenantAdminService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Policy = "Permission:CreateUsers")]
        public async Task<IActionResult> Create(CreateTenantAdminDTO dto, CancellationToken ct)
        {
            var result = await _service.CreateAsync(dto, ct);
            return Ok(result);
        }
    }
}
