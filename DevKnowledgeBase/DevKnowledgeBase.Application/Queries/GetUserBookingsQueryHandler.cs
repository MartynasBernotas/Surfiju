using AutoMapper;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, List<BookingDto>>
    {
        private readonly DevDatabaseContext _context;
        private readonly IMapper _mapper;

        public GetUserBookingsQueryHandler(DevDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Bookings
                .Include(b => b.Camp)
                .Where(b => b.UserId == request.UserId);

            if (request.Status.HasValue)
                query = query.Where(b => b.Status == request.Status.Value);

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<BookingDto>>(bookings);
        }
    }
}
