using Eventix.Application.DTOs.Review;
using Microsoft.AspNetCore.Mvc;
using Eventix.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewController(IReviewService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var res = await _service.GetByIdAsync(id, ct);
        return res == null ? NotFound() : Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewDTO dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));
}