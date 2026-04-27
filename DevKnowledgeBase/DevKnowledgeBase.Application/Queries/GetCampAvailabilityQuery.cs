using DevKnowledgeBase.Domain.Enums;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Queries;

public record GetCampAvailabilityQuery(Guid CampId) : IRequest<CampAvailabilityDto>;

public record CampAvailabilityDto(int MaxParticipants, int BookedParticipants, int AvailableSpots, bool IsAvailable);

public class GetCampAvailabilityQueryHandler(DevDatabaseContext db) : IRequestHandler<GetCampAvailabilityQuery, CampAvailabilityDto>
{
    public async Task<CampAvailabilityDto> Handle(GetCampAvailabilityQuery request, CancellationToken ct)
    {
        var camp = await db.Camps.FindAsync(new object[] { request.CampId }, ct)
            ?? throw new KeyNotFoundException($"Camp {request.CampId} not found.");

        var booked = await db.Bookings
            .Where(b => b.CampId == request.CampId && b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Refunded)
            .SumAsync(b => (int?)b.Participants, ct) ?? 0;

        var available = camp.MaxParticipants - booked;
        return new CampAvailabilityDto(camp.MaxParticipants, booked, available, available > 0);
    }
}
