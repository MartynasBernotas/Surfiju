using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public class DeleteSurfSpotCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteSurfSpotCommand(Guid id)
        {
            Id = id;
        }
    }
}
