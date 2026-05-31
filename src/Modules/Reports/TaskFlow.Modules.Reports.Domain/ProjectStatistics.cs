namespace TaskFlow.Modules.Reports.Domain;

/// <summary>
/// Aggregate task statistics for a single project. <see cref="CompletionRate"/>
/// is a fraction in the range 0..1 (completed ÷ total).
/// </summary>
public sealed record ProjectStatistics(
    Guid ProjectId,
    int TotalTasks,
    int CompletedTasks,
    int OpenTasks,
    double CompletionRate);
