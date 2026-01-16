namespace NotebookTherapy.Application.DTOs;

public class UpdateCategoryDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsActive { get; set; }
}
