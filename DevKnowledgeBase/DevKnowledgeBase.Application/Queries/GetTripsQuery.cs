using AutoMapper;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record GetTripsQuery : IRequest<List<TripDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public GetTripsQuery(int pageNumber, int pageSize, string? search)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Search = search;
    }
}

public class GetTripsQueryHandler : IRequestHandler<GetTripsQuery, List<TripDto>>
{
    private readonly DevDatabaseContext _dbContext;
    private readonly IMapper _mapper;

    public GetTripsQueryHandler(DevDatabaseContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<TripDto>> Handle(GetTripsQuery request, CancellationToken cancellationToken)
    {
        var tripsQuery = _dbContext.Notes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            tripsQuery = tripsQuery.Where(x => x.Title.Contains(request.Search));
        }

        var allTrips = await tripsQuery
            .OrderBy(x => x.Title)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<TripDto>>(allTrips);
    }
}
