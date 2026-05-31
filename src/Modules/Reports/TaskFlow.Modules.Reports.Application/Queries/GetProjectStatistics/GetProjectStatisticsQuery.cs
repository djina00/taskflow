using TaskFlow.Modules.Reports.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Reports.Application.Queries.GetProjectStatistics;

/// <summary>Produces the task statistics for a single project.</summary>
public sealed record GetProjectStatisticsQuery(Guid ProjectId) : IQuery<ProjectReportDto>;
