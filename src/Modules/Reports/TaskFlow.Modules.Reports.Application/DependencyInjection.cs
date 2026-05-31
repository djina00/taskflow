using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Reports.Application.Contracts;
using TaskFlow.Modules.Reports.Application.Queries.GetProjectStatistics;
using TaskFlow.Modules.Reports.Application.Queries.GetUserProductivity;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Reports.Application;

/// <summary>
/// Composition entry point for the Reports module's application layer. Registers
/// the query handlers against their closed-generic interfaces so the query
/// dispatcher can resolve them. The <c>ITaskStatisticsProvider</c> adapter that
/// feeds them is registered separately by the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddReportsModule(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetProjectStatisticsQuery, ProjectReportDto>, GetProjectStatisticsQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserProductivityQuery, UserProductivityDto>, GetUserProductivityQueryHandler>();

        return services;
    }
}
