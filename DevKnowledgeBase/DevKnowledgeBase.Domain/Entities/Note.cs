namespace DevKnowledgeBase.Domain.Entities
{
    public class Note
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get;set; } = new List<string>();
        public DateTime CreatedAt { get; set; }

        public Note()
        {
            Id = Guid.NewGuid();  // Ensure each note gets a unique Id
            Tags = new List<string>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
