using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler : ICommandHandler<UpdateProjectCommand, Result>
{
    private readonly IProjectRepository _projects;

    public UpdateProjectCommandHandler(IProjectRepository projects) => _projects = projects;

    public async Task<Result> HandleAsync(
        UpdateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await _projects.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NotFound($"Project '{command.ProjectId}' was not found."));

        var result = project.UpdateDetails(command.Name ?? string.Empty, command.Description);
        if (result.IsFailure)
            return result;

        await _projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }
}
