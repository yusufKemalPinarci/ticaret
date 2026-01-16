using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto?> GetCategoryBySlugAsync(string slug);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
    Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
    Task<bool> DeleteCategoryAsync(int id);
}
