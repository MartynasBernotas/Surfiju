namespace DevKnowledgeBase.UI.Models
{
    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3,
        Refunded = 4
    }

    public class BookingModel
    {
        public Guid Id { get; set; }
        public Guid CampId { get; set; }
        public Guid UserId { get; set; }
        public int Participants { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public bool CanCancel { get; set; }

        // Additional properties for UI
        public string CampName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
    }

    public class CreateBookingModel
    {
        public Guid CampId { get; set; }
        public int Participants { get; set; }
    }

    public class CreateBookingResponse
    {
        public Guid Id { get; set; }
        public string ClientSecret { get; set; } = string.Empty;
    }
}
