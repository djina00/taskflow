using TaskFlow.Modules.Notifications.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Notifications.Application.Commands.MarkNotificationAsRead;

/// <summary>Loads the notification and lets the aggregate flip it to read.</summary>
public sealed class MarkNotificationAsReadCommandHandler : ICommandHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly INotificationRepository _notifications;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notifications)
        => _notifications = notifications;

    public async Task<Result> HandleAsync(
        MarkNotificationAsReadCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await _notifications.GetByIdAsync(command.NotificationId, cancellationToken);
        if (notification is null)
            return Result.Failure(Error.NotFound($"Notification '{command.NotificationId}' was not found."));

        var readResult = notification.MarkAsRead();
        if (readResult.IsFailure)
            return readResult;

        await _notifications.UpdateAsync(notification, cancellationToken);
        return Result.Success();
    }
}
