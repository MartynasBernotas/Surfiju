using MediatR;
using System;

namespace DevKnowledgeBase.Application.Commands
{
    public class CreateBookingCommand : IRequest<Guid>
    {
        public Guid CampId { get; }
        public string UserId { get; }
        public int Participants { get; }

        public CreateBookingCommand(Guid campId, string userId, int participants)
        {
            CampId = campId;
            UserId = userId;
            Participants = participants;
        }
    }
}
