using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Tasks.Domain;

/// <summary>
/// Persistence port for the <see cref="TaskItem"/> aggregate. Extends the generic
/// <see cref="IRepository{T}"/> with the lookups the Tasks use cases need —
/// listing the tasks of a project or those assigned to a user. Concrete adapters
/// live in the Infrastructure layer and are supplied via Dependency Injection.
/// </summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IReadOnlyList<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid assigneeId, CancellationToken cancellationToken = default);
}
