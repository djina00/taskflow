using TaskFlow.Modules.Notifications.Application.EventHandlers;
using TaskFlow.Modules.Notifications.Domain;
using TaskFlow.SharedKernel.Domain.Events;
using TaskFlow.Tests.Fakes;

namespace TaskFlow.Tests.Application;

/// <summary>
/// Verifies the event-driven seam in isolation: the Notifications module reacts to
/// the cross-module task events (defined in the SharedKernel) without any reference
/// to the Tasks module.
/// </summary>
public class NotificationEventHandlerTests
{
    private readonly InMemoryNotificationRepository _notifications = new();

    [Fact]
    public async Task TaskAssignedEvent_creates_a_notification_for_the_assignee()
    {
        var assignee = Guid.NewGuid();
        var handler = new TaskAssignedNotificationHandler(_notifications);

        await handler.HandleAsync(new TaskAssignedEvent(Guid.NewGuid(), assignee, "Punch the cards"));

        var created = Assert.Single(await _notifications.GetByRecipientAsync(assignee));
        Assert.Equal(NotificationType.TaskAssigned, created.Type);
        Assert.False(created.IsRead);
    }

    [Fact]
    public async Task TaskCompletedEvent_without_an_assignee_creates_nothing()
    {
        var handler = new TaskCompletedNotificationHandler(_notifications);

        await handler.HandleAsync(new TaskCompletedEvent(Guid.NewGuid(), AssigneeId: null, "Orphan task"));

        Assert.Empty(await _notifications.GetAllAsync());
    }
}
