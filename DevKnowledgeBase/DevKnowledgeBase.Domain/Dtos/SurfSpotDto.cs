using DevKnowledgeBase.Domain.Enums;

namespace DevKnowledgeBase.Domain.Dtos
{
    public class SurfSpotDto
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
    }

    public class SurfSpotSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public BreakType BreakType { get; set; }
    }

    public class CreateSurfSpotDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public BreakType BreakType { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public CrowdLevel CrowdLevel { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string>? Photos { get; set; } = new();
    }

    public class UpdateSurfSpotDto
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
    }
}
