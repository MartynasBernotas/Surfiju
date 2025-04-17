namespace DevKnowledgeBase.Domain.Dtos
{
    public class CampDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> PhotoUrls { get; set; } = new();
        public bool IsPublic { get; set; }
        public int CurrentParticipants { get; set; } // Calculated field
        public string OrganizerId { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
    }

    public class CreateCampDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> PhotoUrls { get; set; } = new();
        public bool IsPublic { get; set; }
    }

    public class UpdateCampDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> PhotoUrls { get; set; } = new();
        public bool IsPublic { get; set; } = true;
    }
}
