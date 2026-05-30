using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.AssignTask;

/// <summary>Assigns (or reassigns) a task to a user.</summary>
public sealed record AssignTaskCommand(Guid TaskId, Guid AssigneeId) : ICommand<Result>;
