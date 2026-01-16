using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
{
    public ProductVariantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(int productId)
    {
        return await _dbSet.Where(v => v.ProductId == productId && !v.IsDeleted).ToListAsync();
    }
}
