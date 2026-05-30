using TaskFlow.Modules.Notifications.Domain;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Domain.Events;

namespace TaskFlow.Modules.Notifications.Application.EventHandlers;

/// <summary>
/// Reacts to the cross-module <see cref="TaskCompletedEvent"/> by notifying the
/// task's assignee that it is now done. An unassigned task has nobody to notify,
/// so the handler simply does nothing in that case.
/// </summary>
public sealed class TaskCompletedNotificationHandler : IDomainEventHandler<TaskCompletedEvent>
{
    private readonly INotificationRepository _notifications;

    public TaskCompletedNotificationHandler(INotificationRepository notifications) => _notifications = notifications;

    public async Task HandleAsync(TaskCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent.AssigneeId is not Guid assigneeId)
            return;

        var result = Notification.Create(
            assigneeId,
            NotificationType.TaskCompleted,
            $"Task '{domainEvent.Title}' has been completed.");

        if (result.IsSuccess)
            await _notifications.AddAsync(result.Value, cancellationToken);
    }
}
