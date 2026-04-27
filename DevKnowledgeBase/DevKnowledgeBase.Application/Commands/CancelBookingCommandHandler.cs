using DevKnowledgeBase.Application.Notifications;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly DevDatabaseContext _context;
        private readonly IMediator _mediator;

        public CancelBookingCommandHandler(DevDatabaseContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _context.Bookings
                .Include(b => b.Camp)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.UserId == request.UserId, cancellationToken);

            if (booking == null)
            {
                return false;
            }

            booking.Cancel(request.Reason ?? "Cancelled by user");
            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new BookingCancelledNotification(
                booking.Id,
                booking.User.Email!,
                booking.User.FullName,
                booking.Camp.Name,
                booking.Camp.StartDate,
                booking.Participants,
                booking.TotalPrice,
                booking.CancellationReason), cancellationToken);

            return true;
        }
    }
}
