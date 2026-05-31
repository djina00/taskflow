namespace TaskFlow.Modules.Reports.Domain;

/// <summary>
/// Pure domain logic for the reporting calculations. Deliberately free of any
/// I/O or framework dependency — it takes plain <see cref="TaskSnapshot"/> inputs
/// and returns value objects, which makes the productivity rules trivial to unit
/// test in isolation.
/// </summary>
public static class ProductivityCalculator
{
    /// <summary>Aggregates the supplied tasks (all expected to belong to <paramref name="projectId"/>).</summary>
    public static ProjectStatistics ForProject(Guid projectId, IEnumerable<TaskSnapshot> tasks)
    {
        var snapshots = tasks as IReadOnlyCollection<TaskSnapshot> ?? tasks.ToList();

        var total = snapshots.Count;
        var completed = snapshots.Count(task => task.IsCompleted);
        var completionRate = total == 0 ? 0d : (double)completed / total;

        return new ProjectStatistics(
            projectId,
            total,
            completed,
            total - completed,
            Math.Round(completionRate, 4));
    }

    /// <summary>Computes one user's productivity from the full set of tasks, keeping only those assigned to them.</summary>
    public static UserProductivity ForUser(Guid userId, IEnumerable<TaskSnapshot> tasks)
    {
        var assigned = tasks.Where(task => task.AssigneeId == userId).ToList();
        var completed = assigned.Where(task => task.IsCompleted).ToList();
        var points = completed.Sum(task => task.PriorityWeight);
        var completionRate = assigned.Count == 0 ? 0d : (double)completed.Count / assigned.Count;

        return new UserProductivity(
            userId,
            assigned.Count,
            completed.Count,
            points,
            Math.Round(completionRate, 4));
    }
}
