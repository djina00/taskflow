using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Notifications.Domain;

namespace TaskFlow.Modules.Notifications.Infrastructure;

/// <summary>
/// Composition entry point for the Notifications module's infrastructure layer.
/// Binds the <see cref="INotificationRepository"/> port to its concrete adapter.
/// The module's event-handler subscriptions are registered by <c>AddNotificationsModule</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
