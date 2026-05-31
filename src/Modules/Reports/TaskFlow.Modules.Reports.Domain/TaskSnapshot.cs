namespace TaskFlow.Modules.Reports.Domain;

/// <summary>
/// A minimal, Reports-owned read model of a task — just the facts the reporting
/// calculations need. Deliberately independent of the Tasks module's domain
/// types: the data crosses the module boundary through the
/// <c>ITaskStatisticsProvider</c> port, mapped into this snapshot, so the Reports
/// module never references Tasks. <see cref="PriorityWeight"/> is the task's
/// priority expressed as points (Low = 1, Medium = 2, High = 3).
/// </summary>
public sealed record TaskSnapshot(
    Guid TaskId,
    Guid ProjectId,
    Guid? AssigneeId,
    bool IsCompleted,
    int PriorityWeight);
