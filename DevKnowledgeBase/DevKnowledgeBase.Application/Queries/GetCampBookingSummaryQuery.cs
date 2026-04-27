using DevKnowledgeBase.Domain.Enums;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Queries;

public record GetCampBookingSummaryQuery(string OrganizerId, Guid? CampId) : IRequest<BookingSummaryDto>;

public record BookingSummaryDto(
    decimal TotalRevenue,
    int TotalBookings,
    int ConfirmedBookings,
    int PendingBookings,
    int CancelledBookings);

public class GetCampBookingSummaryQueryHandler(DevDatabaseContext db) : IRequestHandler<GetCampBookingSummaryQuery, BookingSummaryDto>
{
    public async Task<BookingSummaryDto> Handle(GetCampBookingSummaryQuery request, CancellationToken ct)
    {
        var query = db.Bookings
            .Include(b => b.Camp)
            .Where(b => b.Camp.OrganizerId == request.OrganizerId);

        if (request.CampId.HasValue)
            query = query.Where(b => b.CampId == request.CampId.Value);

        var bookings = await query.ToListAsync(ct);

        return new BookingSummaryDto(
            TotalRevenue: bookings.Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed).Sum(b => b.TotalPrice),
            TotalBookings: bookings.Count,
            ConfirmedBookings: bookings.Count(b => b.Status == BookingStatus.Confirmed),
            PendingBookings: bookings.Count(b => b.Status == BookingStatus.Pending),
            CancelledBookings: bookings.Count(b => b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.Refunded));
    }
}
