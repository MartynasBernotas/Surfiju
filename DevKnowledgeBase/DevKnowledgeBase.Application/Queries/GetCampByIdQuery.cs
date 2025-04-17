using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetCampByIdQuery : IRequest<CampDto?>
    {
        public Guid CampId { get; set; }

        public GetCampByIdQuery(Guid campId)
        {
            CampId = campId;
        }
    }
}
