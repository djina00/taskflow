using System.Windows;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Domain.Events;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// The presentation layer's subscription to <see cref="NotificationCreatedEvent"/>.
/// It is the live channel that makes the Notifications tab reactive: when any flow
/// creates a notification, the repository dispatches the event and this handler nudges
/// the singleton <see cref="NotificationsViewModel"/> to reload — no manual refresh.
/// The domain-event dispatch may resume on a background thread, so the nudge is
/// marshalled onto the UI thread before touching the bound collection.
/// </summary>
public sealed class NotificationCreatedUiHandler : IDomainEventHandler<NotificationCreatedEvent>
{
    private readonly NotificationsViewModel _notifications;

    public NotificationCreatedUiHandler(NotificationsViewModel notifications) => _notifications = notifications;

    public Task HandleAsync(NotificationCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            _notifications.OnNotificationCreated(domainEvent.RecipientId);
        else
            dispatcher.InvokeAsync(() => _notifications.OnNotificationCreated(domainEvent.RecipientId));

        return Task.CompletedTask;
    }
}
