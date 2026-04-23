namespace Eventix.Application.DTOs.EventCategories;

public class UpdateEventCategoryDTO
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}