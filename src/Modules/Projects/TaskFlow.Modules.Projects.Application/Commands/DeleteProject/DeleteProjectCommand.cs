using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid ProjectId) : ICommand<Result>;
