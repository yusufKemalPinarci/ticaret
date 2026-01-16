using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync();
    }

    public async Task<Cart?> GetBySessionIdAsync(string sessionId)
    {
        return await _dbSet
            .Where(c => c.SessionId == sessionId && !c.IsDeleted)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync();
    }

    public async Task<Cart?> GetCartWithItemsAsync(int cartId)
    {
        return await _dbSet
            .Where(c => c.Id == cartId && !c.IsDeleted)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Category)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync();
    }

    public async Task<Cart?> GetCartByCartItemIdAsync(int cartItemId)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.Items.Any(i => i.Id == cartItemId))
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Category)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync();
    }
}
