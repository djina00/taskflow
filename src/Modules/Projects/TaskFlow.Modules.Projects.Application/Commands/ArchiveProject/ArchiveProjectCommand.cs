using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.ArchiveProject;

/// <summary>Archives a project, freezing its membership.</summary>
public sealed record ArchiveProjectCommand(Guid ProjectId) : ICommand<Result>;
