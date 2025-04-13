using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class DeleteTripCommand : IRequest<bool>
    {
        public Guid TripId { get; set; }
        public string OrganizerId { get; set; } = string.Empty;

        public DeleteTripCommand(Guid tripId, string organizerId)
        {
            TripId = tripId;
            OrganizerId = organizerId;
        }
    }
}
