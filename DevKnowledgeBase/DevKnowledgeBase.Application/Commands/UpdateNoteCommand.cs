using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands
{
    public record UpdateNoteCommand(Guid Id, string Title, string Content, List<string> Tags) : IRequest<Guid?>;

    public class UpdateNoteHandler : IRequestHandler<UpdateNoteCommand, Guid?>
    {
        private readonly DevDatabaseContext _context;

        public UpdateNoteHandler(DevDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Guid?> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (note is null) { return null; }

            note.Title = request.Title;
            note.Content = request.Content;
            note.Tags = request.Tags;
            _context.Notes.Update(note);
            _context.SaveChanges();

            return note.Id;
        }
    }
}
    
