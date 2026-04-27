using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class CancelBookingCommand : IRequest<bool>
    {
        public Guid BookingId { get; }
        public string UserId { get; }
        public string? Reason { get; }

        public CancelBookingCommand(Guid bookingId, string userId, string? reason = null)
        {
            BookingId = bookingId;
            UserId = userId;
            Reason = reason;
        }
    }
}
