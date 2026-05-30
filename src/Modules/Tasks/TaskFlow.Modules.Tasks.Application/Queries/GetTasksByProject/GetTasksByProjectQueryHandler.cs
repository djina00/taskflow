using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Tasks.Application.Queries.GetTasksByProject;

public sealed class GetTasksByProjectQueryHandler
    : IQueryHandler<GetTasksByProjectQuery, IReadOnlyList<TaskItemDto>>
{
    private readonly ITaskRepository _tasks;

    public GetTasksByProjectQueryHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<IReadOnlyList<TaskItemDto>> HandleAsync(
        GetTasksByProjectQuery query, CancellationToken cancellationToken = default)
    {
        var tasks = await _tasks.GetByProjectAsync(query.ProjectId, cancellationToken);
        return tasks.Select(task => task.ToDto()).ToArray();
    }
}
