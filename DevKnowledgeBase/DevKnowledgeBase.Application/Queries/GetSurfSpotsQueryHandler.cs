using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class GetSurfSpotsQueryHandler : IRequestHandler<GetSurfSpotsQuery, List<SurfSpotDto>>
    {
        private readonly DevDatabaseContext _context;

        public GetSurfSpotsQueryHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<SurfSpotDto>> Handle(GetSurfSpotsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.SurfSpots.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Region))
                query = query.Where(s => EF.Functions.Like(s.Location, $"%{request.Region.Trim()}%"));

            if (request.SkillLevel.HasValue)
                query = query.Where(s => s.SkillLevel == request.SkillLevel.Value);

            if (request.BreakType.HasValue)
                query = query.Where(s => s.BreakType == request.BreakType.Value);

            var spots = await query.OrderBy(s => s.Name).ToListAsync(cancellationToken);

            return spots.Select(s => new SurfSpotDto
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                BreakType = s.BreakType,
                SkillLevel = s.SkillLevel,
                CrowdLevel = s.CrowdLevel,
                Description = s.Description,
                Photos = s.Photos
            }).ToList();
        }
    }
}
