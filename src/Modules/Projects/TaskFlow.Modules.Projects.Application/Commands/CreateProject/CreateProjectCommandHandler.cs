using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.CreateProject;

/// <summary>
/// Delegates the invariant checks and state change to the <see cref="Project"/>
/// aggregate, then persists the new project.
/// </summary>
public sealed class CreateProjectCommandHandler
    : ICommandHandler<CreateProjectCommand, Result<ProjectDto>>
{
    private readonly IProjectRepository _projects;

    public CreateProjectCommandHandler(IProjectRepository projects) => _projects = projects;

    public async Task<Result<ProjectDto>> HandleAsync(
        CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var result = Project.Create(command.Name ?? string.Empty, command.OwnerId, command.Description);
        if (result.IsFailure)
            return result.Error;

        await _projects.AddAsync(result.Value, cancellationToken);
        return result.Value.ToDto();
    }
}
