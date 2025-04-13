using AutoMapper;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Queries
{
    public record GetAllNotesQuery : IRequest<List<NoteDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public GetAllNotesQuery(int pageNumber, int pageSize, string? search)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Search = search;
        }
    };

    public class GetAllNotesHandler : IRequestHandler<GetAllNotesQuery, List<NoteDto>>
    {
        private readonly DevDatabaseContext _dbContext;
        private readonly IMapper _mapper;

        public GetAllNotesHandler(DevDatabaseContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<NoteDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            var notesQuery = _dbContext.Notes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                notesQuery = notesQuery.Where(x => x.Title.Contains(request.Search));
            }

            var allNotes = await notesQuery
                .OrderBy(x => x.Title)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<NoteDto>>(allNotes);
        }
    }
}
