using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Domain.Enums;
using MediatR;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetSurfSpotsQuery : IRequest<List<SurfSpotDto>>
    {
        public string? Region { get; }
        public SkillLevel? SkillLevel { get; }
        public BreakType? BreakType { get; }

        public GetSurfSpotsQuery(string? region = null, SkillLevel? skillLevel = null, BreakType? breakType = null)
        {
            Region = region;
            SkillLevel = skillLevel;
            BreakType = breakType;
        }
    }
}
