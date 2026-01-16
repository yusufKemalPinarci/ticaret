using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Where(c => c.Slug == slug && c.IsActive && !c.IsDeleted)
            .Include(c => c.Products)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive && !c.IsDeleted)
            .Include(c => c.Products)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet
            .Where(c => !c.IsDeleted)
            .Include(c => c.Products)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeCategoryId = null)
    {
        return await _dbSet.AnyAsync(c => c.Slug == slug && !c.IsDeleted && (!excludeCategoryId.HasValue || c.Id != excludeCategoryId));
    }
}
