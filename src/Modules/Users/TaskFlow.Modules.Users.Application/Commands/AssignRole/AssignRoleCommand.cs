using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Commands.AssignRole;

/// <summary>Grants a named role from the catalogue to an existing user.</summary>
public sealed record AssignRoleCommand(Guid UserId, string RoleName) : ICommand<Result>;
