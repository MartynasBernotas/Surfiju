using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class DeleteTripCommandHandler : IRequestHandler<DeleteTripCommand, bool>
    {
        private readonly DevDatabaseContext _context;

        public DeleteTripCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips.FindAsync(request.TripId);
            if (trip == null)
            {
                return false;
            }

            // Verify that the organizer is authorized to delete this trip
            if (trip.OrganizerId != request.OrganizerId)
            {
                return false;
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
