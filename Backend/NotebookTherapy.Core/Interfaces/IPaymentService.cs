using NotebookTherapy.Core.Entities;
using System.Threading.Tasks;

namespace NotebookTherapy.Core.Interfaces;

public interface IPaymentService
{
    Task<(string paymentIntentId, string clientSecret, string status)> CreateOrUpdatePaymentIntentAsync(Order order, string currency);

    Task<string> RefundPaymentAsync(string paymentIntentId, decimal amount);

    Task<string> CancelPaymentIntentAsync(string paymentIntentId);
}
