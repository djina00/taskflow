using TaskFlow.Modules.Reports.Domain;

namespace TaskFlow.Modules.Reports.Application.Contracts;

/// <summary>
/// Maps the Reports domain value objects to their outward-facing DTOs. Centralised
/// here so every use case projects reports identically.
/// </summary>
internal static class ReportMappingExtensions
{
    public static ProjectReportDto ToDto(this ProjectStatistics statistics) => new(
        statistics.ProjectId,
        statistics.TotalTasks,
        statistics.CompletedTasks,
        statistics.OpenTasks,
        statistics.CompletionRate);

    public static UserProductivityDto ToDto(this UserProductivity productivity) => new(
        productivity.UserId,
        productivity.AssignedTasks,
        productivity.CompletedTasks,
        productivity.Points,
        productivity.CompletionRate);
}
