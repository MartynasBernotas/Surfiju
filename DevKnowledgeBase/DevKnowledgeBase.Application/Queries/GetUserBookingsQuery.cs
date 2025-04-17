using DevKnowledgeBase.Domain.Dtos;
using MediatR;
using System.Collections.Generic;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetUserBookingsQuery : IRequest<List<BookingDto>>
    {
        public string UserId { get; }

        public GetUserBookingsQuery(string userId)
        {
            UserId = userId;
        }
    }
}
