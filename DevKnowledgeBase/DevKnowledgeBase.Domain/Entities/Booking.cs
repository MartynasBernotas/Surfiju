using DevKnowledgeBase.Domain.Enums;

namespace DevKnowledgeBase.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid CampId { get; set; }
        public string UserId { get; set; }
        public int Participants { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; private set; } = BookingStatus.Pending;
        public string? PaymentIntentId { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime? CancelledAt { get; private set; }

        // Navigation properties
        public Camp Camp { get; set; } = null!;
        public User User { get; set; } = null!;

        public void Confirm(string paymentIntentId)
        {
            Status = BookingStatus.Confirmed;
            PaymentIntentId = paymentIntentId;
        }

        public void Cancel(string reason)
        {
            Status = Status == BookingStatus.Confirmed ? BookingStatus.Refunded : BookingStatus.Cancelled;
            CancellationReason = reason;
            CancelledAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be completed.");
            Status = BookingStatus.Completed;
        }
    }
}
