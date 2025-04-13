using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Queries
{
    public record GetNoteByIdQuery : IRequest<NoteDto?>
    {
        public Guid Id { get; set; }
        public GetNoteByIdQuery(Guid id) { Id = id; }
    }

    public class GetNoteByIdHandler : IRequestHandler<GetNoteByIdQuery, NoteDto?>
    {
        private readonly DevDatabaseContext _dbContext;
        public GetNoteByIdHandler(DevDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<NoteDto?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == request.Id);
            return note is null ? null : new NoteDto(note.Id, note.Title, note.Content, note.Tags);
        }
    }
}
