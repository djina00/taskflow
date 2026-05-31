using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Reports.Application.Abstractions;

namespace TaskFlow.Modules.Reports.Infrastructure;

/// <summary>
/// Composition entry point for the Reports module's infrastructure layer. Binds the
/// <see cref="ITaskStatisticsProvider"/> port to the adapter that reads the Tasks
/// store. The query handlers themselves are registered by <c>AddReportsModule</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddReportsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaskStatisticsProvider, TaskStatisticsProvider>();

        return services;
    }
}
