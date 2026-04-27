using DevKnowledgeBase.Application.Services;
using MediatR;

namespace DevKnowledgeBase.Application.Notifications.Handlers;

public class BookingCancelledEmailHandler(IEmailService emailService) : INotificationHandler<BookingCancelledNotification>
{
    public async Task Handle(BookingCancelledNotification n, CancellationToken ct)
    {
        var body = $"""
            <h2>Booking Cancelled</h2>
            <p>Hi {n.UserName},</p>
            <p>Your booking for <strong>{n.CampName}</strong> starting {n.StartDate:MMMM dd, yyyy} has been cancelled.</p>
            <p>Participants: {n.Participants} | Total: €{n.TotalPrice:F2}</p>
            {(n.CancellationReason != null ? $"<p>Reason: {n.CancellationReason}</p>" : "")}
            """;
        await emailService.SendEmailAsync(n.UserEmail, "Your Surfiju Booking has been Cancelled", body);
    }
}
