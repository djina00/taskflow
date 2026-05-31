namespace TaskFlow.Modules.Reports.Application.Contracts;

/// <summary>
/// Read model for a user's productivity. <see cref="Points"/> sums the priority
/// weights of completed tasks; <see cref="CompletionRate"/> is a fraction 0..1.
/// </summary>
public sealed record UserProductivityDto(
    Guid UserId,
    int AssignedTasks,
    int CompletedTasks,
    int Points,
    double CompletionRate);
