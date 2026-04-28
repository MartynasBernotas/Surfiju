using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands
{
    public class UpdateSurfSpotCommandHandler : IRequestHandler<UpdateSurfSpotCommand, bool>
    {
        private readonly DevDatabaseContext _context;

        public UpdateSurfSpotCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateSurfSpotCommand request, CancellationToken cancellationToken)
        {
            var spot = await _context.SurfSpots.FirstOrDefaultAsync(s => s.Id == request.Dto.Id, cancellationToken);
            if (spot == null) return false;

            spot.Name = request.Dto.Name;
            spot.Location = request.Dto.Location;
            spot.Latitude = request.Dto.Latitude;
            spot.Longitude = request.Dto.Longitude;
            spot.BreakType = request.Dto.BreakType;
            spot.SkillLevel = request.Dto.SkillLevel;
            spot.CrowdLevel = request.Dto.CrowdLevel;
            spot.Description = request.Dto.Description;
            spot.Photos = request.Dto.Photos;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
