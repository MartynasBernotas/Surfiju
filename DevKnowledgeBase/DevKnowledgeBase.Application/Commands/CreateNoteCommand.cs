using DevKnowledgeBase.Application.Events;
using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public record CreateNoteCommand : IRequest<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class CreateNoteHandler : IRequestHandler<CreateNoteCommand, Guid>
    {
        private readonly DevDatabaseContext _context;
        private readonly IMediator _mediator;

        public CreateNoteHandler(DevDatabaseContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            var note = new Note
            {
                Title = request.Title,
                Content = request.Content,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new NewNoteCreatedEvent(note.Id), cancellationToken);

            return note.Id;
        }
    }
}
