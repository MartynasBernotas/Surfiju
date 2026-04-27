using DevKnowledgeBase.Application.Notifications;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands;

public record CompleteBookingCommand(Guid BookingId) : IRequest;

public class CompleteBookingCommandHandler(DevDatabaseContext db, IMediator mediator) : IRequestHandler<CompleteBookingCommand>
{
    public async Task Handle(CompleteBookingCommand request, CancellationToken ct)
    {
        var booking = await db.Bookings
            .Include(b => b.Camp)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, ct)
            ?? throw new KeyNotFoundException($"Booking {request.BookingId} not found.");

        booking.Complete();
        await db.SaveChangesAsync(ct);

        await mediator.Publish(new BookingCompletedNotification(
            booking.Id,
            booking.User.Email!,
            booking.User.FullName,
            booking.Camp.Name,
            booking.Camp.StartDate,
            booking.Participants,
            booking.TotalPrice), ct);
    }
}
