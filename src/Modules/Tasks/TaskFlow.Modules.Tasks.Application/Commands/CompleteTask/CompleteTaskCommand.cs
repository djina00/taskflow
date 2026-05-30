using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.CompleteTask;

/// <summary>Marks a task as completed.</summary>
public sealed record CompleteTaskCommand(Guid TaskId) : ICommand<Result>;
