namespace TaskFlow.Modules.Reports.Application.Contracts;

/// <summary>
/// Read model for a project's task statistics. <see cref="CompletionRate"/> is a
/// fraction in the range 0..1.
/// </summary>
public sealed record ProjectReportDto(
    Guid ProjectId,
    int TotalTasks,
    int CompletedTasks,
    int OpenTasks,
    double CompletionRate);
