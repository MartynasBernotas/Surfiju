using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class GetCampByIdQueryHandler : IRequestHandler<GetCampByIdQuery, CampDto?>
    {
        private readonly DevDatabaseContext _context;

        public GetCampByIdQueryHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<CampDto?> Handle(GetCampByIdQuery request, CancellationToken cancellationToken)
        {
            var camp = await _context.Camps
                .Include(t => t.Organizer)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == request.CampId, cancellationToken);

            if (camp == null)
            {
                return null;
            }

            return new CampDto
            {
                Id = camp.Id,
                Name = camp.Name,
                StartDate = camp.StartDate,
                EndDate = camp.EndDate,
                Description = camp.Description,
                MaxParticipants = camp.MaxParticipants,
                Price = camp.Price,
                Location = camp.Location,
                PhotoUrls = camp.PhotoUrls,
                IsPublic = camp.IsPublic,
                CurrentParticipants = camp.Members.Count,
                OrganizerId = camp.OrganizerId,
                OrganizerName = camp.Organizer.FullName
            };
        }
    }
}
