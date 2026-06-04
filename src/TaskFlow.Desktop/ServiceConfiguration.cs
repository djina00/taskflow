using System.IO;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Desktop.ViewModels;
using TaskFlow.Infrastructure;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Domain.Events;
using TaskFlow.Modules.Notifications.Application;
using TaskFlow.Modules.Notifications.Infrastructure;
using TaskFlow.Modules.Projects.Application;
using TaskFlow.Modules.Projects.Infrastructure;
using TaskFlow.Modules.Reports.Application;
using TaskFlow.Modules.Reports.Infrastructure;
using TaskFlow.Modules.Tasks.Application;
using TaskFlow.Modules.Tasks.Infrastructure;
using TaskFlow.Modules.Users.Application;
using TaskFlow.Modules.Users.Infrastructure;

namespace TaskFlow.Desktop;

/// <summary>
/// The composition root of the modular monolith. Here — and only here — every
/// module's application use cases and infrastructure adapters are assembled into a
/// single dependency-injection container, alongside the shared infrastructure
/// (dispatchers and the chosen persistence adapter). Each module contributes
/// through its own <c>AddXModule</c> / <c>AddXInfrastructure</c> extensions, so the
/// modules stay independent and this file reads as a table of contents for the app.
/// </summary>
public static class ServiceConfiguration
{
    public static IServiceProvider Build(string contentRoot)
    {
        var services = new ServiceCollection();

        var dataDirectory = Path.Combine(contentRoot, "data");
        services.AddInfrastructure(PersistenceKind.Json, dataDirectory);

        // Application layer: command/query/event handlers for every module.
        services
            .AddUsersModule()
            .AddProjectsModule()
            .AddTasksModule()
            .AddNotificationsModule()
            .AddReportsModule();

        // Infrastructure layer: repositories and adapters for every module.
        services
            .AddUsersInfrastructure()
            .AddProjectsInfrastructure()
            .AddTasksInfrastructure()
            .AddNotificationsInfrastructure()
            .AddReportsInfrastructure();

        // Presentation layer: shared session, feature view models and the windows.
        services.AddSingleton<SessionContext>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<RegisterViewModel>();
        services.AddSingleton<ProjectsViewModel>();
        services.AddSingleton<TasksViewModel>();
        services.AddSingleton<NotificationsViewModel>();
        services.AddSingleton<ReportsViewModel>();
        services.AddSingleton<MainViewModel>();

        // Live UI channel: the presentation layer subscribes to notification creation
        // so the Notifications tab updates reactively instead of on manual refresh.
        services.AddSingleton<IDomainEventHandler<NotificationCreatedEvent>, NotificationCreatedUiHandler>();

        // Windows are transient so sign-out/sign-in can open a fresh one (a closed
        // WPF window cannot be shown again).
        services.AddTransient<LoginWindow>();
        services.AddTransient<RegisterWindow>();
        services.AddTransient<MainWindow>();

        return services.BuildServiceProvider();
    }
}
