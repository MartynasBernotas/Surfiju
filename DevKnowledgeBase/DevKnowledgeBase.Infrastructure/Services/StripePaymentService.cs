using DevKnowledgeBase.Application.Interfaces;
using Stripe;

namespace DevKnowledgeBase.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency, Guid bookingId)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency,
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", bookingId.ToString() }
            }
        };
        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);
        return intent.ClientSecret;
    }

    public async Task<bool> RefundPaymentAsync(string paymentIntentId, decimal amount)
    {
        var options = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId,
            Amount = (long)(amount * 100)
        };
        var service = new RefundService();
        var refund = await service.CreateAsync(options);
        return refund.Status == "succeeded" || refund.Status == "pending";
    }
}
