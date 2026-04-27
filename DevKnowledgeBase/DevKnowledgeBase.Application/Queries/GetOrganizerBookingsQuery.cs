using DevKnowledgeBase.Domain.Enums;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Queries;

public record GetOrganizerBookingsQuery(string OrganizerId, Guid? CampId, BookingStatus? Status) : IRequest<List<OrganizerBookingDto>>;

public record OrganizerBookingDto(
    Guid BookingId,
    string CampName,
    string UserName,
    string UserEmail,
    int Participants,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime BookingDate,
    string? PaymentIntentId);

public class GetOrganizerBookingsQueryHandler(DevDatabaseContext db) : IRequestHandler<GetOrganizerBookingsQuery, List<OrganizerBookingDto>>
{
    public async Task<List<OrganizerBookingDto>> Handle(GetOrganizerBookingsQuery request, CancellationToken ct)
    {
        var query = db.Bookings
            .Include(b => b.Camp)
            .Include(b => b.User)
            .Where(b => b.Camp.OrganizerId == request.OrganizerId);

        if (request.CampId.HasValue)
            query = query.Where(b => b.CampId == request.CampId.Value);

        if (request.Status.HasValue)
            query = query.Where(b => b.Status == request.Status.Value);

        return await query
            .OrderByDescending(b => b.BookingDate)
            .Select(b => new OrganizerBookingDto(
                b.Id,
                b.Camp.Name,
                b.User.FullName,
                b.User.Email!,
                b.Participants,
                b.TotalPrice,
                b.Status,
                b.BookingDate,
                b.PaymentIntentId))
            .ToListAsync(ct);
    }
}
