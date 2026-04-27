using MediatR;

namespace DevKnowledgeBase.Application.Notifications;

public record BookingCancelledNotification(
    Guid BookingId,
    string UserEmail,
    string UserName,
    string CampName,
    DateTime StartDate,
    int Participants,
    decimal TotalPrice,
    string? CancellationReason) : INotification;
