using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.ArchiveProject;

/// <summary>
/// Loads the project and lets the aggregate perform the lifecycle transition.
/// Persists only when the project was actually archived.
/// </summary>
public sealed class ArchiveProjectCommandHandler : ICommandHandler<ArchiveProjectCommand, Result>
{
    private readonly IProjectRepository _projects;

    public ArchiveProjectCommandHandler(IProjectRepository projects) => _projects = projects;

    public async Task<Result> HandleAsync(
        ArchiveProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await _projects.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NotFound($"Project '{command.ProjectId}' was not found."));

        var archiveResult = project.Archive();
        if (archiveResult.IsFailure)
            return archiveResult;

        await _projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }
}
