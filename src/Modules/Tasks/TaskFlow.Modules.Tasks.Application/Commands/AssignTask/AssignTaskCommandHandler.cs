using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.AssignTask;

/// <summary>
/// Loads the task and lets the aggregate enforce the assignment rules (a
/// completed task cannot be reassigned). The resulting <c>TaskAssignedEvent</c>
/// is published by the domain-event dispatcher after persistence.
/// </summary>
public sealed class AssignTaskCommandHandler : ICommandHandler<AssignTaskCommand, Result>
{
    private readonly ITaskRepository _tasks;

    public AssignTaskCommandHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<Result> HandleAsync(
        AssignTaskCommand command, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
            return Result.Failure(Error.NotFound($"Task '{command.TaskId}' was not found."));

        var assignResult = task.Assign(command.AssigneeId);
        if (assignResult.IsFailure)
            return assignResult;

        await _tasks.UpdateAsync(task, cancellationToken);
        return Result.Success();
    }
}
