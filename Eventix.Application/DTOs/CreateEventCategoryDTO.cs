namespace Eventix.Application.DTOs.EventCategories;

public class CreateEventCategoryDTO
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}