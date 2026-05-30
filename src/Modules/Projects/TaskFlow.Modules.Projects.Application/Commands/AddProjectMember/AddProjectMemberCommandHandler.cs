using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.AddProjectMember;

/// <summary>
/// Resolves the project and the requested role, then lets the aggregate decide
/// whether the membership is valid. Persists only when the aggregate accepts it.
/// </summary>
public sealed class AddProjectMemberCommandHandler : ICommandHandler<AddProjectMemberCommand, Result>
{
    private readonly IProjectRepository _projects;

    public AddProjectMemberCommandHandler(IProjectRepository projects) => _projects = projects;

    public async Task<Result> HandleAsync(
        AddProjectMemberCommand command, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<ProjectRole>(command.Role, ignoreCase: true, out var role)
            || !Enum.IsDefined(role))
            return Result.Failure(Error.Validation($"'{command.Role}' is not a valid project role."));

        var project = await _projects.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NotFound($"Project '{command.ProjectId}' was not found."));

        var addResult = project.AddMember(command.UserId, role);
        if (addResult.IsFailure)
            return addResult;

        await _projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }
}
