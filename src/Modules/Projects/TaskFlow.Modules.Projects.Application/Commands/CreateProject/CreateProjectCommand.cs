using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.CreateProject;

/// <summary>Creates a new project owned by the given user.</summary>
public sealed record CreateProjectCommand(string Name, Guid OwnerId, string? Description)
    : ICommand<Result<ProjectDto>>;
