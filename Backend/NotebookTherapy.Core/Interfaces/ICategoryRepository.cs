using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<bool> SlugExistsAsync(string slug, int? excludeCategoryId = null);
}
