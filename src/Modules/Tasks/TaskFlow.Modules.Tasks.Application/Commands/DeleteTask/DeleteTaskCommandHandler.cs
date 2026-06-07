using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.DeleteTask;

public sealed class DeleteTaskCommandHandler : ICommandHandler<DeleteTaskCommand, Result>
{
    private readonly ITaskRepository _tasks;

    public DeleteTaskCommandHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<Result> HandleAsync(
        DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
            return Result.Failure(Error.NotFound($"Task '{command.TaskId}' was not found."));

        await _tasks.DeleteAsync(command.TaskId, cancellationToken);
        return Result.Success();
    }
}
