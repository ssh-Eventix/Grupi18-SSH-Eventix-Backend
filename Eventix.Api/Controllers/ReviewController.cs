using Eventix.Application.DTOs.Review;
using Eventix.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [Authorize(Policy = "Permission:ViewReviews")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission:ViewReviews")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var res = await _service.GetByIdAsync(id, ct);
        return res == null ? NotFound() : Ok(res);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:CreateReviews")]
    public async Task<IActionResult> Create(CreateReviewDTO dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));


}