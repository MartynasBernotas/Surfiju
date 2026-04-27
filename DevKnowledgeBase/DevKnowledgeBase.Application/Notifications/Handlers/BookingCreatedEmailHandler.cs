using DevKnowledgeBase.Application.Services;
using MediatR;

namespace DevKnowledgeBase.Application.Notifications.Handlers;

public class BookingCreatedEmailHandler(IEmailService emailService) : INotificationHandler<BookingCreatedNotification>
{
    public async Task Handle(BookingCreatedNotification n, CancellationToken ct)
    {
        var body = $"""
            <h2>Booking Received</h2>
            <p>Hi {n.UserName}, your booking for <strong>{n.CampName}</strong> starting {n.StartDate:MMMM dd, yyyy} is pending payment.</p>
            <p>Participants: {n.Participants} | Total: €{n.TotalPrice:F2}</p>
            """;
        await emailService.SendEmailAsync(n.UserEmail, "Your Surfiju Booking is Pending", body);
    }
}
