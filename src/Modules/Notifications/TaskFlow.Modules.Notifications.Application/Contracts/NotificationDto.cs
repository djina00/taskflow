namespace TaskFlow.Modules.Notifications.Application.Contracts;

/// <summary>
/// Read model exposed by the Notifications use cases. Projects the notification
/// type as its name so consumers never depend on the domain enumeration.
/// </summary>
public sealed record NotificationDto(
    Guid Id,
    Guid RecipientId,
    string Type,
    string Message,
    bool IsRead,
    DateTime CreatedOnUtc);
