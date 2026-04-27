using MediatR;

namespace DevKnowledgeBase.Application.Notifications;

public record BookingConfirmedNotification(
    Guid BookingId,
    string UserEmail,
    string UserName,
    string CampName,
    DateTime StartDate,
    int Participants,
    decimal TotalPrice) : INotification;
