using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand, Result>
{
    private readonly ITaskRepository _tasks;

    public UpdateTaskCommandHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<Result> HandleAsync(
        UpdateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var priority = TaskPriority.Medium;
        if (!string.IsNullOrWhiteSpace(command.Priority)
            && (!Enum.TryParse(command.Priority, ignoreCase: true, out priority) || !Enum.IsDefined(priority)))
            return Result.Failure(Error.Validation($"'{command.Priority}' is not a valid task priority."));

        var task = await _tasks.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
            return Result.Failure(Error.NotFound($"Task '{command.TaskId}' was not found."));

        var result = task.UpdateDetails(command.Title ?? string.Empty, command.Description, priority);
        if (result.IsFailure)
            return result;

        await _tasks.UpdateAsync(task, cancellationToken);
        return Result.Success();
    }
}
