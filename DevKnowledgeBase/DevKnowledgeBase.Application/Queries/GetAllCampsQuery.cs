using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetAllCampsQuery : IRequest<PaginatedQueryResult<CampDto>>
    {
        public bool IncludeInactive { get; set; }
        public string? OrganizerId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public string? Location { get; set; }

        public GetAllCampsQuery(int pageNumber, int pageSizer, string? location, bool includeInactive = false, string? organizerId = null)
        {
            IncludeInactive = includeInactive;
            OrganizerId = organizerId;
            PageNumber = pageNumber;
            PageSize = pageSizer;
            if (!string.IsNullOrWhiteSpace(location))
            {
                Location = location;
            }
        }
    }
}
