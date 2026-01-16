using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<Order?> GetOrderWithItemsAsync(int orderId);
    Task<Order?> GetByPaymentIntentIdAsync(string paymentIntentId);
}
