using DevKnowledgeBase.Domain.Dtos;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class UpdateSurfSpotCommand : IRequest<bool>
    {
        public UpdateSurfSpotDto Dto { get; }

        public UpdateSurfSpotCommand(UpdateSurfSpotDto dto)
        {
            Dto = dto;
        }
    }
}
