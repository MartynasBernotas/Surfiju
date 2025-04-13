using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetTripByIdQuery : IRequest<TripDto?>
    {
        public Guid TripId { get; set; }

        public GetTripByIdQuery(Guid tripId)
        {
            TripId = tripId;
        }
    }
}
