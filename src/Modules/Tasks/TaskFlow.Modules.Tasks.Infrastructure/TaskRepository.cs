using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Tasks.Infrastructure;

/// <summary>
/// Persistence adapter for the <see cref="TaskItem"/> aggregate. Reuses the generic
/// <see cref="DocumentRepository{T}"/> for CRUD — including the publishing of the
/// cross-module task events on save — and adds the project/assignee lookups the use
/// cases need.
/// </summary>
public sealed class TaskRepository : DocumentRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(IDocumentStore store, IDomainEventDispatcher dispatcher)
        : base(store, dispatcher) { }

    public async Task<IReadOnlyList<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        return all.Where(task => task.ProjectId == projectId).ToList();
    }

    public async Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid assigneeId, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        return all.Where(task => task.AssigneeId == assigneeId).ToList();
    }
}
