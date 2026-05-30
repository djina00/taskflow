using TaskFlow.Modules.Notifications.Domain;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Domain.Events;

namespace TaskFlow.Modules.Notifications.Application.EventHandlers;

/// <summary>
/// Reacts to the cross-module <see cref="TaskAssignedEvent"/> by creating a
/// notification for the assignee. The Notifications module subscribes purely
/// through the SharedKernel contract — it has no reference to the Tasks module,
/// which is what keeps the two modules loosely coupled and independently
/// deployable. This is the event-driven seam of the architecture.
/// </summary>
public sealed class TaskAssignedNotificationHandler : IDomainEventHandler<TaskAssignedEvent>
{
    private readonly INotificationRepository _notifications;

    public TaskAssignedNotificationHandler(INotificationRepository notifications) => _notifications = notifications;

    public async Task HandleAsync(TaskAssignedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var result = Notification.Create(
            domainEvent.AssigneeId,
            NotificationType.TaskAssigned,
            $"You have been assigned to task '{domainEvent.Title}'.");

        if (result.IsSuccess)
            await _notifications.AddAsync(result.Value, cancellationToken);
    }
}
