using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IWishlistRepository : IRepository<WishlistItem>
{
    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId);
    Task<WishlistItem?> GetByUserAndProductAsync(int userId, int productId);
}
