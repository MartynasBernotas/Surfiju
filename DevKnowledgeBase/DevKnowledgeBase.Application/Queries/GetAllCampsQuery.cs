using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetAllCampsQuery : IRequest<List<CampDto>>
    {
        public bool IncludeInactive { get; set; }
        public string? OrganizerId { get; set; }

        public GetAllCampsQuery(bool includeInactive = false, string? organizerId = null)
        {
            IncludeInactive = includeInactive;
            OrganizerId = organizerId;
        }
    }
}
