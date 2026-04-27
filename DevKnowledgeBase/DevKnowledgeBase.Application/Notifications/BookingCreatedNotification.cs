using MediatR;

namespace DevKnowledgeBase.Application.Notifications;

public record BookingCreatedNotification(
    Guid BookingId,
    string UserEmail,
    string UserName,
    string CampName,
    DateTime StartDate,
    int Participants,
    decimal TotalPrice) : INotification;
