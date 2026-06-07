using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid TaskId) : ICommand<Result>;
