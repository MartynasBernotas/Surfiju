using System;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class CancelBookingCommand : IRequest<bool>
    {
        public Guid BookingId { get; }
        public string UserId { get; }

        public CancelBookingCommand(Guid bookingId, string userId)
        {
            BookingId = bookingId;
            UserId = userId;
        }
    }
}
