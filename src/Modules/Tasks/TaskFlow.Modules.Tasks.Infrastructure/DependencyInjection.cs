using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Tasks.Domain;

namespace TaskFlow.Modules.Tasks.Infrastructure;

/// <summary>
/// Composition entry point for the Tasks module's infrastructure layer. Binds the
/// <see cref="ITaskRepository"/> port to its concrete adapter.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddTasksInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
    }
}
