using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class GetTripByIdQueryHandler : IRequestHandler<GetTripByIdQuery, TripDto?>
    {
        private readonly DevDatabaseContext _context;

        public GetTripByIdQueryHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<TripDto?> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
        {
            var trip = await _context.Trips
                .Include(t => t.Organizer)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

            if (trip == null)
            {
                return null;
            }

            return new TripDto
            {
                Id = trip.Id,
                Name = trip.Name,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Description = trip.Description,
                MaxParticipants = trip.MaxParticipants,
                Price = trip.Price,
                Location = trip.Location,
                PhotoUrls = trip.PhotoUrls,
                IsPublic = trip.IsPublic,
                CurrentParticipants = trip.Members.Count,
                OrganizerId = trip.OrganizerId,
                OrganizerName = trip.Organizer.FullName
            };
        }
    }
}
