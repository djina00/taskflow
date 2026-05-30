using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.CompleteTask;

/// <summary>
/// Loads the task and lets the aggregate mark it complete. The resulting
/// <c>TaskCompletedEvent</c> is published by the domain-event dispatcher after
/// persistence, allowing other modules (Notifications, Reports) to react.
/// </summary>
public sealed class CompleteTaskCommandHandler : ICommandHandler<CompleteTaskCommand, Result>
{
    private readonly ITaskRepository _tasks;

    public CompleteTaskCommandHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<Result> HandleAsync(
        CompleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
            return Result.Failure(Error.NotFound($"Task '{command.TaskId}' was not found."));

        var completeResult = task.Complete();
        if (completeResult.IsFailure)
            return completeResult;

        await _tasks.UpdateAsync(task, cancellationToken);
        return Result.Success();
    }
}
