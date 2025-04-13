using Azure.Core;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands
{
    public record DeleteNoteCommand(Guid Id) : IRequest<bool>;

    public class DeleteNoteHandler : IRequestHandler<DeleteNoteCommand,bool>
    {
        private readonly DevDatabaseContext _dbContext;
        public DeleteNoteHandler(DevDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteNoteCommand command, CancellationToken cancellationToken)
        {
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == command.Id);
            if (note is null) return false;

            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
