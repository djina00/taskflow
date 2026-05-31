using TaskFlow.Modules.Tasks.Domain;

namespace TaskFlow.Tests.Fakes;

public sealed class InMemoryTaskRepository : InMemoryRepository<TaskItem>, ITaskRepository
{
    public Task<IReadOnlyList<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        Task.FromResult((IReadOnlyList<TaskItem>)Items.Where(task => task.ProjectId == projectId).ToList());

    public Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid assigneeId, CancellationToken cancellationToken = default) =>
        Task.FromResult((IReadOnlyList<TaskItem>)Items.Where(task => task.AssigneeId == assigneeId).ToList());
}
