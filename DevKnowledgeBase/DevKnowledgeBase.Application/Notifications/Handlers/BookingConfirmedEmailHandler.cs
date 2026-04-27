using DevKnowledgeBase.Application.Services;
using MediatR;

namespace DevKnowledgeBase.Application.Notifications.Handlers;

public class BookingConfirmedEmailHandler(IEmailService emailService) : INotificationHandler<BookingConfirmedNotification>
{
    public async Task Handle(BookingConfirmedNotification n, CancellationToken ct)
    {
        var body = $"""
            <h2>Booking Confirmed!</h2>
            <p>Hi {n.UserName},</p>
            <p>Your booking for <strong>{n.CampName}</strong> starting {n.StartDate:MMMM dd, yyyy} is confirmed.</p>
            <p>Participants: {n.Participants} | Total: €{n.TotalPrice:F2}</p>
            """;
        await emailService.SendEmailAsync(n.UserEmail, "Your Surfiju Booking is Confirmed", body);
    }
}
