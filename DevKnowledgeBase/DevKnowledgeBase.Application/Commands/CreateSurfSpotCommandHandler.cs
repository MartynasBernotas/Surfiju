using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class CreateSurfSpotCommandHandler : IRequestHandler<CreateSurfSpotCommand, Guid>
    {
        private readonly DevDatabaseContext _context;

        public CreateSurfSpotCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateSurfSpotCommand request, CancellationToken cancellationToken)
        {
            var spot = new SurfSpot
            {
                Name = request.Dto.Name,
                Location = request.Dto.Location,
                Latitude = request.Dto.Latitude,
                Longitude = request.Dto.Longitude,
                BreakType = request.Dto.BreakType,
                SkillLevel = request.Dto.SkillLevel,
                CrowdLevel = request.Dto.CrowdLevel,
                Description = request.Dto.Description,
                Photos = request.Dto.Photos
            };

            _context.SurfSpots.Add(spot);
            await _context.SaveChangesAsync(cancellationToken);
            return spot.Id;
        }
    }
}
