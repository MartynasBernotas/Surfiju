// DevKnowledgeBase.Domain/Dtos/BookingDto.cs
using System;

namespace DevKnowledgeBase.Domain.Dtos
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid CampId { get; set; }
        public string UserId { get; set; }
        public int Participants { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        
        // Additional properties to simplify UI display
        public string CampName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
    }
}
