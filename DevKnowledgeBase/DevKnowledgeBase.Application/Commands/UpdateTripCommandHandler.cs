using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class UpdateTripCommandHandler : IRequestHandler<UpdateTripCommand, bool>
    {
        private readonly DevDatabaseContext _context;

        public UpdateTripCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips.FindAsync(request.Id);
            if (trip == null)
            {
                return false;
            }

            // Verify that the organizer is authorized to update this trip
            if (trip.OrganizerId != request.OrganizerId)
            {
                return false;
            }

            trip.Name = request.Name;
            trip.StartDate = request.StartDate.ToUniversalTime();
            trip.EndDate = request.EndDate.ToUniversalTime();
            trip.Description = request.Description;
            trip.MaxParticipants = request.MaxParticipants;
            trip.Price = request.Price;
            trip.Location = request.Location;
            trip.PhotoUrls = request.PhotoUrls;
            trip.IsPublic = request.IsPublic;

            _context.Update(trip);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
