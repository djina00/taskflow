using TaskFlow.Modules.Reports.Application.Abstractions;
using TaskFlow.Modules.Reports.Domain;
using TaskFlow.Modules.Tasks.Domain;

namespace TaskFlow.Modules.Reports.Infrastructure;

/// <summary>
/// Adapter that satisfies the Reports module's <see cref="ITaskStatisticsProvider"/>
/// port by reading the Tasks store and projecting each <see cref="TaskItem"/> into a
/// Reports-owned <see cref="TaskSnapshot"/>. This is the only place the two modules
/// meet: the mapping lives in the Infrastructure layer, so the Reports use cases and
/// domain remain free of any dependency on Tasks.
/// </summary>
public sealed class TaskStatisticsProvider : ITaskStatisticsProvider
{
    private readonly ITaskRepository _tasks;

    public TaskStatisticsProvider(ITaskRepository tasks) => _tasks = tasks;

    public async Task<IReadOnlyList<TaskSnapshot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _tasks.GetAllAsync(cancellationToken);
        return tasks.Select(ToSnapshot).ToList();
    }

    public async Task<IReadOnlyList<TaskSnapshot>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await _tasks.GetByProjectAsync(projectId, cancellationToken);
        return tasks.Select(ToSnapshot).ToList();
    }

    // TaskPriority is defined as Low = 1, Medium = 2, High = 3 — exactly the point
    // weights the productivity calculation expects.
    private static TaskSnapshot ToSnapshot(TaskItem task) => new(
        task.Id,
        task.ProjectId,
        task.AssigneeId,
        task.IsCompleted,
        (int)task.Priority);
}
