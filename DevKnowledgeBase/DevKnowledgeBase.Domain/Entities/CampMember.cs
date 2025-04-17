namespace DevKnowledgeBase.Domain.Entities
{
    public class CampMember
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Camp Camp { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; }
    }
}
