using DevKnowledgeBase.Application.Notifications;
using DevKnowledgeBase.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Application.Commands;

public record ConfirmBookingCommand(Guid BookingId, string PaymentIntentId) : IRequest;

public class ConfirmBookingCommandHandler(DevDatabaseContext db, IMediator mediator) : IRequestHandler<ConfirmBookingCommand>
{
    public async Task Handle(ConfirmBookingCommand request, CancellationToken ct)
    {
        var booking = await db.Bookings
            .Include(b => b.Camp)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, ct)
            ?? throw new KeyNotFoundException($"Booking {request.BookingId} not found.");

        booking.Confirm(request.PaymentIntentId);
        await db.SaveChangesAsync(ct);

        await mediator.Publish(new BookingConfirmedNotification(
            booking.Id,
            booking.User.Email!,
            booking.User.FullName,
            booking.Camp.Name,
            booking.Camp.StartDate,
            booking.Participants,
            booking.TotalPrice), ct);
    }
}
