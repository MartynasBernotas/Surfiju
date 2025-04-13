using DevKnowledgeBase.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevKnowledgeBase.Application.Events
{
    public record NewNoteCreatedEvent : INotification
    {
        public Guid NoteId { get; }

        public NewNoteCreatedEvent(Guid noteId)
        {
            NoteId = noteId;
        }
    }

    public class NewNoteCreatedEventHandler : INotificationHandler<NewNoteCreatedEvent>
    {
        private readonly ILogger<NewNoteCreatedEventHandler> _logger;

        public NewNoteCreatedEventHandler(ILogger<NewNoteCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(NewNoteCreatedEvent notification, CancellationToken cancellation)
        {
            _logger.LogInformation($"✅ New note created with ID: {notification.NoteId}");
            await Task.FromResult( notification );
        }
    }
}
