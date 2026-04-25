using AutoMapper;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record GetCampsQuery : IRequest<List<CampDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public GetCampsQuery(int pageNumber, int pageSize, string? search)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Search = search;
    }
}

public class GetCampsQueryHandler : IRequestHandler<GetCampsQuery, List<CampDto>>
{
    private readonly DevDatabaseContext _dbContext;
    private readonly IMapper _mapper;

    public GetCampsQueryHandler(DevDatabaseContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<CampDto>> Handle(GetCampsQuery request, CancellationToken cancellationToken)
    {
        var campsQuery = _dbContext.Camps.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            campsQuery = campsQuery.Where(x => x.Name.Contains(request.Search));
        }

        var allCamps = await campsQuery
            .OrderBy(x => x.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<CampDto>>(allCamps);
    }
}
