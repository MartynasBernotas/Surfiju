using System.ComponentModel.DataAnnotations;

namespace DevKnowledgeBase.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }
        public decimal Price { get; set; }
        public List<string>? PhotoUrls { get; set; } = new();
        public bool IsPublic { get; set; } = true;
        public string OrganizerId { get; set; } = string.Empty;
        public User Organizer { get; set; } = null!;
        public ICollection<TripMember> Members { get; set; } = new List<TripMember>();

        //public List<TripFeature> Features { get; set; } = new(); // Lessons, Transport, etc.

        public Trip()
        {
            Id = Guid.NewGuid(); 
        }
    }
}
