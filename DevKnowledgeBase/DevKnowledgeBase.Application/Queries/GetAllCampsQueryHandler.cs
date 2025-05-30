using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class GetAllCampsQueryHandler : IRequestHandler<GetAllCampsQuery, PaginatedQueryResult<CampDto>>
    {
        private readonly DevDatabaseContext _context;

        public GetAllCampsQueryHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<PaginatedQueryResult<CampDto>> Handle(GetAllCampsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Camps.Include(t => t.Organizer).Include(t => t.Members).AsQueryable();
            try
            {
                if (!string.IsNullOrWhiteSpace(request.Location))
                {
                    var normalizedLocation = request.Location.Trim();
                    query = query.Where(t => t.Location != null &&
                                              EF.Functions.Like(t.Location, $"%{normalizedLocation}%"));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            

            if (!request.IncludeInactive)
                query = query.Where(t => t.IsPublic);


            if (request.OrganizerId is not null)
                query = query.Where(t => t.OrganizerId == request.OrganizerId);

            var totalCount = await query.CountAsync(cancellationToken);

            var paginatedCamps = await query
               .OrderBy(x => x.StartDate)
               .Skip((request.PageNumber - 1) * request.PageSize)
               .Take(request.PageSize)
               .ToListAsync(cancellationToken);

            var dtos = paginatedCamps.Select(ToDto).ToList();

            return new PaginatedQueryResult<CampDto>(dtos, totalCount);
        }

        private CampDto ToDto(Camp t)
        {
            return new CampDto
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
            };
        }
    }
}
