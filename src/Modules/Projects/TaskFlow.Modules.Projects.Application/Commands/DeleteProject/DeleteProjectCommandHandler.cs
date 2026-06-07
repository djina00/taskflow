using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand, Result>
{
    private readonly IProjectRepository _projects;

    public DeleteProjectCommandHandler(IProjectRepository projects) => _projects = projects;

    public async Task<Result> HandleAsync(
        DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await _projects.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NotFound($"Project '{command.ProjectId}' was not found."));

        await _projects.DeleteAsync(command.ProjectId, cancellationToken);
        return Result.Success();
    }
}
