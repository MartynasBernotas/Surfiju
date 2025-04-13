namespace DevKnowledgeBase.UI.Models
{
    public class TripModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> PhotoUrls { get; set; } = new();
        public int CurrentParticipants { get; set; }
        public Guid OrganizerId { get; set; } = Guid.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
    }
}
