using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetSurfSpotByIdQuery : IRequest<SurfSpotDto?>
    {
        public Guid Id { get; }

        public GetSurfSpotByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
