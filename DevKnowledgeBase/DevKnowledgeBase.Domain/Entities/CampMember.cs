namespace DevKnowledgeBase.Domain.Entities
{
    public class CampMember
    {
        public Guid Id { get; set; }
        public Guid CampId { get; set; }
        public Camp Camp { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}
