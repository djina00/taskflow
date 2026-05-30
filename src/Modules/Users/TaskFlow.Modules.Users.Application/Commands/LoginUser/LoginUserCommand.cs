using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Commands.LoginUser;

/// <summary>
/// Authenticates a user with email and password. Modelled as a command rather
/// than a query because authentication is an intentful action that, in a real
/// system, would carry side effects (issuing a session/token, audit logging).
/// </summary>
public sealed record LoginUserCommand(string Email, string Password)
    : ICommand<Result<UserDto>>;
