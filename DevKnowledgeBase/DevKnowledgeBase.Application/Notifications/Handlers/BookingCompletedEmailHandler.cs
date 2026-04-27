using DevKnowledgeBase.Application.Services;
using MediatR;

namespace DevKnowledgeBase.Application.Notifications.Handlers;

public class BookingCompletedEmailHandler(IEmailService emailService) : INotificationHandler<BookingCompletedNotification>
{
    public async Task Handle(BookingCompletedNotification n, CancellationToken ct)
    {
        var body = $"""
            <h2>Booking Completed</h2>
            <p>Hi {n.UserName},</p>
            <p>Your booking for <strong>{n.CampName}</strong> starting {n.StartDate:MMMM dd, yyyy} is now complete. We hope you had an amazing time!</p>
            <p>Participants: {n.Participants} | Total: €{n.TotalPrice:F2}</p>
            """;
        await emailService.SendEmailAsync(n.UserEmail, "Your Surfiju Booking is Complete", body);
    }
}
