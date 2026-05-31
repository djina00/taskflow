namespace TaskFlow.Modules.Reports.Domain;

/// <summary>
/// A user's productivity across all projects. <see cref="Points"/> is the sum of
/// the priority weights of the tasks the user has completed, so finishing
/// higher-priority work scores more. <see cref="CompletionRate"/> is a fraction
/// in the range 0..1 (completed ÷ assigned).
/// </summary>
public sealed record UserProductivity(
    Guid UserId,
    int AssignedTasks,
    int CompletedTasks,
    int Points,
    double CompletionRate);
