using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler
    : IQueryHandler<GetTaskByIdQuery, Result<TaskItemDto>>
{
    private readonly ITaskRepository _tasks;

    public GetTaskByIdQueryHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<Result<TaskItemDto>> HandleAsync(
        GetTaskByIdQuery query, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdAsync(query.TaskId, cancellationToken);
        return task is null
            ? Error.NotFound($"Task '{query.TaskId}' was not found.")
            : task.ToDto();
    }
}
