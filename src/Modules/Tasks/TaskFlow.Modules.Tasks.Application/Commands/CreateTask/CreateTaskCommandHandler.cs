using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.CreateTask;

/// <summary>
/// Resolves the requested priority, then delegates the invariant checks and state
/// change to the <see cref="TaskItem"/> aggregate before persisting the new task.
/// </summary>
public sealed class CreateTaskCommandHandler
    : ICommandHandler<CreateTaskCommand, Result<TaskItemDto>>
{
    private readonly ITaskRepository _tasks;

    public CreateTaskCommandHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<Result<TaskItemDto>> HandleAsync(
        CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var priority = TaskPriority.Medium;
        if (!string.IsNullOrWhiteSpace(command.Priority)
            && (!Enum.TryParse(command.Priority, ignoreCase: true, out priority) || !Enum.IsDefined(priority)))
            return Error.Validation($"'{command.Priority}' is not a valid task priority.");

        var result = TaskItem.Create(command.ProjectId, command.Title ?? string.Empty, command.Description, priority);
        if (result.IsFailure)
            return result.Error;

        await _tasks.AddAsync(result.Value, cancellationToken);
        return result.Value.ToDto();
    }
}
