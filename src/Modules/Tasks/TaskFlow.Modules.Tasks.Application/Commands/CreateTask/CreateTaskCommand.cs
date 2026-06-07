using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.CreateTask;

/// <summary>
/// Creates a new task in a project, reported by <paramref name="ReporterId"/>.
/// Priority defaults to Medium when omitted.
/// </summary>
public sealed record CreateTaskCommand(Guid ProjectId, string Title, string? Description, string? Priority, Guid ReporterId)
    : ICommand<Result<TaskItemDto>>;
