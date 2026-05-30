using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Notifications.Application.Commands.MarkNotificationAsRead;
using TaskFlow.Modules.Notifications.Application.Contracts;
using TaskFlow.Modules.Notifications.Application.EventHandlers;
using TaskFlow.Modules.Notifications.Application.Queries.GetNotificationsForRecipient;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Domain.Events;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Notifications.Application;

/// <summary>
/// Composition entry point for the Notifications module's application layer.
/// Crucially, it registers this module's <see cref="IDomainEventHandler{TEvent}"/>
/// subscriptions against the cross-module task events — this is how the module
/// plugs into the event-driven flow without the Tasks module knowing it exists.
/// The domain-event dispatcher (Infrastructure, step 9) resolves these handlers
/// and invokes them when a task event is published. The repository adapter is
/// registered separately by the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
    {
        // Event-driven subscriptions to other modules' events.
        services.AddScoped<IDomainEventHandler<TaskAssignedEvent>, TaskAssignedNotificationHandler>();
        services.AddScoped<IDomainEventHandler<TaskCompletedEvent>, TaskCompletedNotificationHandler>();

        // This module's own read/write use cases.
        services.AddScoped<ICommandHandler<MarkNotificationAsReadCommand, Result>, MarkNotificationAsReadCommandHandler>();
        services.AddScoped<IQueryHandler<GetNotificationsForRecipientQuery, IReadOnlyList<NotificationDto>>, GetNotificationsForRecipientQueryHandler>();

        return services;
    }
}
