using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface ICheckoutSessionRepository : IRepository<CheckoutSession>
{
    Task<CheckoutSession?> GetByUserAndKeyAsync(int userId, string idempotencyKey);
    Task<CheckoutSession?> GetByOrderIdAsync(int orderId);
}
