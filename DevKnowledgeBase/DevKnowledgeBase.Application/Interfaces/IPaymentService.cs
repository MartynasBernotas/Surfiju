namespace DevKnowledgeBase.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(decimal amount, string currency, Guid bookingId);
    Task<bool> RefundPaymentAsync(string paymentIntentId, decimal amount);
}
