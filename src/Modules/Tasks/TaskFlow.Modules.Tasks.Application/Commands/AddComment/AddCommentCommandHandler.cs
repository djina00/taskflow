using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.AddComment;

/// <summary>Loads the task and lets the aggregate append a comment after validating it.</summary>
public sealed class AddCommentCommandHandler : ICommandHandler<AddCommentCommand, Result>
{
    private readonly ITaskRepository _tasks;

    public AddCommentCommandHandler(ITaskRepository tasks) => _tasks = tasks;

    public async Task<Result> HandleAsync(
        AddCommentCommand command, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
            return Result.Failure(Error.NotFound($"Task '{command.TaskId}' was not found."));

        var commentResult = task.AddComment(command.AuthorId, command.Text ?? string.Empty);
        if (commentResult.IsFailure)
            return commentResult;

        await _tasks.UpdateAsync(task, cancellationToken);
        return Result.Success();
    }
}
