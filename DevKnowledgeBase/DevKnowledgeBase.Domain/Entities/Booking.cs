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

        // Navigation properties
        public Camp Camp { get; set; } = null!; // Fix: Use null-forgiving operator to suppress the warning
        public User User { get; set; } = null!; // Fix: Use null-forgiving operator to suppress the warning
    }
}
