using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetAllTripsQuery : IRequest<List<TripDto>>
    {
        public bool IncludeInactive { get; set; }
        public string? OrganizerId { get; set; }

        public GetAllTripsQuery(bool includeInactive = false, string? organizerId = null)
        {
            IncludeInactive = includeInactive;
            OrganizerId = organizerId;
        }
    }
}
