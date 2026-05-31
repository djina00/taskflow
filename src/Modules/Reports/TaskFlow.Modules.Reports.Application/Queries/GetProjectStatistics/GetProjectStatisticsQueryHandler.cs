using TaskFlow.Modules.Reports.Application.Abstractions;
using TaskFlow.Modules.Reports.Application.Contracts;
using TaskFlow.Modules.Reports.Domain;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Reports.Application.Queries.GetProjectStatistics;

/// <summary>
/// Pulls the project's task snapshots through the port and runs them through the
/// pure <see cref="ProductivityCalculator"/> domain logic. An unknown project
/// simply yields zeroed statistics.
/// </summary>
public sealed class GetProjectStatisticsQueryHandler
    : IQueryHandler<GetProjectStatisticsQuery, ProjectReportDto>
{
    private readonly ITaskStatisticsProvider _statistics;

    public GetProjectStatisticsQueryHandler(ITaskStatisticsProvider statistics) => _statistics = statistics;

    public async Task<ProjectReportDto> HandleAsync(
        GetProjectStatisticsQuery query, CancellationToken cancellationToken = default)
    {
        var tasks = await _statistics.GetByProjectAsync(query.ProjectId, cancellationToken);
        return ProductivityCalculator.ForProject(query.ProjectId, tasks).ToDto();
    }
}
