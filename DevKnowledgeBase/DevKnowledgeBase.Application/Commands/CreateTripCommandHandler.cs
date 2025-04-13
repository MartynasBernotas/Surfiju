using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Handlers
{
    public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Guid>
    {
        private readonly DevDatabaseContext _context;

        public CreateTripCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var trip = new Trip
            {
                Name = request.Name,
                StartDate = request.StartDate.ToUniversalTime(),
                EndDate = request.EndDate.ToUniversalTime(),
                Description = request.Description,
                MaxParticipants = request.MaxParticipants,
                Price = request.Price,
                Location = request.Location,
                PhotoUrls = request.PhotoUrls,
                OrganizerId = request.OrganizerId,
                IsPublic = true
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync(cancellationToken);

            return trip.Id;
        }
    }
}
