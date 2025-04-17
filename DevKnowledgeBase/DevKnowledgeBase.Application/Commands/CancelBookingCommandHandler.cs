using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DevKnowledgeBase.Application.Commands
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly DevDatabaseContext _context;

        public CancelBookingCommandHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.UserId == request.UserId, cancellationToken);

            if (booking == null)
            {
                return false;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
