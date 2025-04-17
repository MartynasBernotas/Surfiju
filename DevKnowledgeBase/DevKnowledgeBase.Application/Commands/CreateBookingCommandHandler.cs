using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevKnowledgeBase.Application.Commands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
    {
        private readonly DevDatabaseContext _context;

        public CreateBookingCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // Get the camp to check availability and price
            var camp = await _context.Camps
                .FirstOrDefaultAsync(c => c.Id == request.CampId, cancellationToken);

            if (camp == null)
            {
                throw new Exception($"Camp with ID {request.CampId} not found.");
            }

            // Check if the camp is published
            if (!camp.IsPublic)
            {
                throw new Exception("This camp is not available for booking.");
            }

            // Check if there are enough spots available
            var currentBookings = await _context.Bookings
                .Where(b => b.CampId == request.CampId)
                .SumAsync(b => b.Participants, cancellationToken);

            int availableSpots = camp.MaxParticipants - currentBookings;

            if (request.Participants > availableSpots)
            {
                throw new Exception($"Not enough spots available. Only {availableSpots} spots left.");
            }

            // Calculate total price
            decimal totalPrice = camp.Price * request.Participants;

            // Create the booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CampId = request.CampId,
                UserId = request.UserId,
                Participants = request.Participants,
                TotalPrice = totalPrice,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }
    }
}
