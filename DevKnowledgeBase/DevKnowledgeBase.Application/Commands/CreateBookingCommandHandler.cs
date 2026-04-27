using DevKnowledgeBase.Application.Interfaces;
using DevKnowledgeBase.Application.Notifications;
using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Domain.Enums;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingResult>
    {
        private readonly DevDatabaseContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IMediator _mediator;

        public CreateBookingCommandHandler(DevDatabaseContext context, IPaymentService paymentService, IMediator mediator)
        {
            _context = context;
            _paymentService = paymentService;
            _mediator = mediator;
        }

        public async Task<CreateBookingResult> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var camp = await _context.Camps
                .FirstOrDefaultAsync(c => c.Id == request.CampId, cancellationToken);

            if (camp == null)
                throw new KeyNotFoundException($"Camp with ID {request.CampId} not found.");

            if (!camp.IsPublic)
                throw new InvalidOperationException("This camp is not available for booking.");

            var booked = await _context.Bookings
                .Where(b => b.CampId == request.CampId && b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Refunded)
                .SumAsync(b => b.Participants, cancellationToken);

            int availableSpots = camp.MaxParticipants - booked;

            if (request.Participants > availableSpots)
                throw new InvalidOperationException($"Not enough spots available. Only {availableSpots} spots left.");

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CampId = request.CampId,
                UserId = request.UserId,
                Participants = request.Participants,
                TotalPrice = camp.Price * request.Participants,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            var clientSecret = await _paymentService.CreatePaymentIntentAsync(booking.TotalPrice, "eur", booking.Id);

            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user != null)
            {
                await _mediator.Publish(new BookingCreatedNotification(
                    booking.Id,
                    user.Email!,
                    user.FullName,
                    camp.Name,
                    camp.StartDate,
                    booking.Participants,
                    booking.TotalPrice), cancellationToken);
            }

            return new CreateBookingResult(booking.Id, clientSecret);
        }
    }
}
