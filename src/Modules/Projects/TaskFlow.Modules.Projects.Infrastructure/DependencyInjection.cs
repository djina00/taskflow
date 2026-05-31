using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Projects.Domain;

namespace TaskFlow.Modules.Projects.Infrastructure;

/// <summary>
/// Composition entry point for the Projects module's infrastructure layer. Binds
/// the <see cref="IProjectRepository"/> port to its concrete adapter.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddProjectsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();

        return services;
    }
}
