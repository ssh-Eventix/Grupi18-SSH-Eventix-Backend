namespace Eventix.Application.DTOs.EventCategories;

public class CreateEventCategoryDTO
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}