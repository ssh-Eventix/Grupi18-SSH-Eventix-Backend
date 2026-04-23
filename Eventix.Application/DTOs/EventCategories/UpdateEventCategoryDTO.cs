namespace Eventix.Application.DTOs.EventCategories;

public class UpdateEventCategoryDTO
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}