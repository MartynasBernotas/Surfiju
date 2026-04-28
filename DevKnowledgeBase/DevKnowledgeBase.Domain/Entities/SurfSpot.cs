using DevKnowledgeBase.Domain.Enums;

namespace DevKnowledgeBase.Domain.Entities
{
    public class SurfSpot
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public BreakType BreakType { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public CrowdLevel CrowdLevel { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string>? Photos { get; set; } = new();
        public ICollection<CampSurfSpot> CampSurfSpots { get; set; } = new List<CampSurfSpot>();

        public SurfSpot() { Id = Guid.NewGuid(); }
    }
}
