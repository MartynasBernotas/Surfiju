using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class GetSurfSpotByIdQueryHandler : IRequestHandler<GetSurfSpotByIdQuery, SurfSpotDto?>
    {
        private readonly DevDatabaseContext _context;

        public GetSurfSpotByIdQueryHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<SurfSpotDto?> Handle(GetSurfSpotByIdQuery request, CancellationToken cancellationToken)
        {
            var spot = await _context.SurfSpots.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (spot == null) return null;

            return new SurfSpotDto
            {
                Id = spot.Id,
                Name = spot.Name,
                Location = spot.Location,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                BreakType = spot.BreakType,
                SkillLevel = spot.SkillLevel,
                CrowdLevel = spot.CrowdLevel,
                Description = spot.Description,
                Photos = spot.Photos
            };
        }
    }
}
