using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class CreateSurfSpotCommand : IRequest<Guid>
    {
        public CreateSurfSpotDto Dto { get; }

        public CreateSurfSpotCommand(CreateSurfSpotDto dto)
        {
            Dto = dto;
        }
    }
}
