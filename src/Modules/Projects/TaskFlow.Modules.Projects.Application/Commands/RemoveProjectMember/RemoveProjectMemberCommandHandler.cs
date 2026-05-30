using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.RemoveProjectMember;

/// <summary>
/// Loads the project and lets the aggregate enforce the removal rules (owner is
/// protected, member must exist). Persists only when the change is accepted.
/// </summary>
public sealed class RemoveProjectMemberCommandHandler : ICommandHandler<RemoveProjectMemberCommand, Result>
{
    private readonly IProjectRepository _projects;

    public RemoveProjectMemberCommandHandler(IProjectRepository projects) => _projects = projects;

    public async Task<Result> HandleAsync(
        RemoveProjectMemberCommand command, CancellationToken cancellationToken = default)
    {
        var project = await _projects.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NotFound($"Project '{command.ProjectId}' was not found."));

        var removeResult = project.RemoveMember(command.UserId);
        if (removeResult.IsFailure)
            return removeResult;

        await _projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }
}
