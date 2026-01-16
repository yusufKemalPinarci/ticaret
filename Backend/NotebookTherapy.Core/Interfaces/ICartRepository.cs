using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart?> GetBySessionIdAsync(string sessionId);
    Task<Cart?> GetCartWithItemsAsync(int cartId);
    Task<Cart?> GetCartByCartItemIdAsync(int cartItemId);
}
