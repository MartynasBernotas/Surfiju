namespace DevKnowledgeBase.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Guid UserId { get; set; }
        public int Participants { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
