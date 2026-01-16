using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;

namespace NotebookTherapy.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;

    public StripePaymentService(IOptions<StripeSettings> settings)
    {
        _settings = settings.Value;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<(string paymentIntentId, string clientSecret, string status)> CreateOrUpdatePaymentIntentAsync(Order order, string currency)
    {
        var service = new PaymentIntentService();
        var amount = (long)Math.Round(order.TotalAmount * 100m, MidpointRounding.AwayFromZero);
        if (amount < 0) amount = 0;

        PaymentIntent intent;
        if (!string.IsNullOrEmpty(order.PaymentIntentId))
        {
            intent = await service.UpdateAsync(order.PaymentIntentId, new PaymentIntentUpdateOptions
            {
                Amount = amount
            });
        }
        else
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };
            intent = await service.CreateAsync(options);
        }

        return (intent.Id, intent.ClientSecret, intent.Status);
    }

    public async Task<string> RefundPaymentAsync(string paymentIntentId, decimal amount)
    {
        var refundService = new RefundService();
        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId,
            Amount = (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero)
        };
        var refund = await refundService.CreateAsync(refundOptions);
        return refund.Status;
    }

    public async Task<string> CancelPaymentIntentAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        var intent = await service.CancelAsync(paymentIntentId);
        return intent.Status;
    }
}

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}
