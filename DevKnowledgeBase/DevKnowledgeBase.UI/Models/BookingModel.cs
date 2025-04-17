// DevKnowledgeBase.UI/Models/BookingModel.cs
namespace DevKnowledgeBase.UI.Models
{
    public class BookingModel
    {
        public Guid Id { get; set; }
        public Guid CampId { get; set; }
        public Guid UserId { get; set; }
        public int Participants { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        
        // Additional properties for UI
        public string CampName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    
    public class CreateBookingModel
    {
        public Guid CampId { get; set; }
        public int Participants { get; set; }
    }
}
