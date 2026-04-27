using MediatR;

namespace DevKnowledgeBase.Application.Notifications;

public record BookingCompletedNotification(
    Guid BookingId,
    string UserEmail,
    string UserName,
    string CampName,
    DateTime StartDate,
    int Participants,
    decimal TotalPrice) : INotification;
