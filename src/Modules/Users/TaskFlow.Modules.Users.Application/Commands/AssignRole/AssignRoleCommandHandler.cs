using TaskFlow.Modules.Users.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Commands.AssignRole;

/// <summary>
/// Resolves the requested role and user, then lets the aggregate decide whether
/// the assignment is valid. Persists only when the aggregate accepts the change.
/// </summary>
public sealed class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand, Result>
{
    private readonly IUserRepository _users;

    public AssignRoleCommandHandler(IUserRepository users) => _users = users;

    public async Task<Result> HandleAsync(
        AssignRoleCommand command, CancellationToken cancellationToken = default)
    {
        var roleResult = Role.FromName(command.RoleName);
        if (roleResult.IsFailure)
            return Result.Failure(roleResult.Error);

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound($"User '{command.UserId}' was not found."));

        var assignResult = user.AssignRole(roleResult.Value);
        if (assignResult.IsFailure)
            return assignResult;

        await _users.UpdateAsync(user, cancellationToken);
        return Result.Success();
    }
}
