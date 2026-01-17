using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class WishlistRepository : Repository<WishlistItem>, IWishlistRepository
{
    public WishlistRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId)
    {
        return await _context.WishlistItems
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task<WishlistItem?> GetByUserAndProductAsync(int userId, int productId)
    {
        return await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
    }
}
