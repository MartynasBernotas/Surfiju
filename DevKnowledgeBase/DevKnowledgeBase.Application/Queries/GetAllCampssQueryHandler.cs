using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class GetAllCampsQueryHandler : IRequestHandler<GetAllCampsQuery, List<CampDto>>
    {
        private readonly DevDatabaseContext _context;

        public GetAllCampsQueryHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<CampDto>> Handle(GetAllCampsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Camps.Include(t => t.Organizer).Include(t => t.Members).AsQueryable();

            if (!request.IncludeInactive)
            {
                query = query.Where(t => t.IsPublic);
            }

            if (request.OrganizerId != null)
            {
                query = query.Where(t => t.OrganizerId == request.OrganizerId);
            }

            return await query.Select(t => new CampDto
            {
                Id = t.Id,
                Name = t.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Description = t.Description,
                MaxParticipants = t.MaxParticipants,
                Price = t.Price,
                Location = t.Location,
                PhotoUrls = t.PhotoUrls,
                IsPublic = t.IsPublic,
                CurrentParticipants = t.Members.Count,
                OrganizerId = t.OrganizerId,
                OrganizerName = t.Organizer.FullName
            }).ToListAsync(cancellationToken);
        }
    }
}
