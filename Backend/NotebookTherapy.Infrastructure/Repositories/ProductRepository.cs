using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Core.Models;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsFeatured && !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetNewProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsNew && !p.IsDeleted)
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBackInStockProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsBackInStock && !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCollectionAsync(string collection)
    {
        return await _dbSet
            .Where(p => p.Collection == collection && !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && 
                   (p.Name.Contains(searchTerm) || 
                    p.Description.Contains(searchTerm) ||
                    p.SKU.Contains(searchTerm)))
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<PagedResult<Product>> GetFilteredProductsAsync(ProductFilterOptions options)
    {
        var page = options.Page < 1 ? 1 : options.Page;
        var pageSize = options.PageSize < 1 ? 1 : options.PageSize > 100 ? 100 : options.PageSize;

        var query = _dbSet
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            var term = options.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) ||
                                     p.Description.ToLower().Contains(term) ||
                                     p.SKU.ToLower().Contains(term));
        }

        if (options.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == options.CategoryId);

        if (!string.IsNullOrWhiteSpace(options.Collection))
            query = query.Where(p => p.Collection == options.Collection);

        if (options.IsFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == options.IsFeatured);

        if (options.IsNew.HasValue)
            query = query.Where(p => p.IsNew == options.IsNew);

        if (options.IsBackInStock.HasValue)
            query = query.Where(p => p.IsBackInStock == options.IsBackInStock);

        if (options.InStockOnly == true)
            query = query.Where(p => p.Stock > 0);

        if (options.HasDiscount == true)
            query = query.Where(p => p.DiscountPrice != null && p.DiscountPrice < p.Price);

        if (options.MinPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) >= options.MinPrice.Value);

        if (options.MaxPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) <= options.MaxPrice.Value);

        query = options.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.Price),
            "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            "featured" => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Where(p => p.Id == id && !p.IsDeleted)
            .Include(p => p.Category)
            .FirstOrDefaultAsync();
    }
}
