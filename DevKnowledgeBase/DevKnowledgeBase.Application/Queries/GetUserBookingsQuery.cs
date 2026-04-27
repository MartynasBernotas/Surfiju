using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace DevKnowledgeBase.Application.Queries
{
    public class GetUserBookingsQuery : IRequest<List<BookingDto>>
    {
        public string UserId { get; }
        public BookingStatus? Status { get; }

        public GetUserBookingsQuery(string userId, BookingStatus? status = null)
        {
            UserId = userId;
            Status = status;
        }
    }
}
