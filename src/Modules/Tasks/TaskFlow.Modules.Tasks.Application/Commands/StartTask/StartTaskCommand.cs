using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.StartTask;

/// <summary>Moves a task from Todo into the InProgress state.</summary>
public sealed record StartTaskCommand(Guid TaskId) : ICommand<Result>;
