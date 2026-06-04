using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Domain.Events;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Notifications.Domain;

/// <summary>
/// A message addressed to a user, created in reaction to something that happened
/// elsewhere in the system (e.g. a task being assigned or completed). The
/// Notifications module owns this aggregate; it knows only a recipient's id and a
/// rendered message, never the originating module's domain types.
/// </summary>
public sealed class Notification : AggregateRoot
{
    private Notification(Guid id, Guid recipientId, NotificationType type, string message, DateTime createdOnUtc)
        : base(id)
    {
        RecipientId = recipientId;
        Type = type;
        Message = message;
        CreatedOnUtc = createdOnUtc;
    }

    /// <summary>Parameterless constructor for serializers.</summary>
    private Notification() { }

    public Guid RecipientId { get; private set; }

    public NotificationType Type { get; private set; }

    public string Message { get; private set; } = string.Empty;

    public bool IsRead { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>Factory for a brand-new, unread notification.</summary>
    public static Result<Notification> Create(Guid recipientId, NotificationType type, string message)
    {
        if (recipientId == Guid.Empty)
            return Error.Validation("A recipient is required.");
        if (string.IsNullOrWhiteSpace(message))
            return Error.Validation("A notification message is required.");

        var notification = new Notification(Guid.NewGuid(), recipientId, type, message.Trim(), DateTime.UtcNow);
        notification.Raise(new NotificationCreatedEvent(notification.Id, recipientId));
        return notification;
    }

    /// <summary>Marks the notification as read. Idempotency is treated as a conflict.</summary>
    public Result MarkAsRead()
    {
        if (IsRead)
            return Result.Failure(Error.Conflict("The notification is already marked as read."));

        IsRead = true;
        return Result.Success();
    }
}
