using TaskFlow.Modules.Users.Application.Abstractions;
using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.Modules.Users.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Commands.LoginUser;

/// <summary>
/// Verifies credentials. Returns the same opaque <c>Unauthorized</c> error
/// whether the email is unknown or the password is wrong, so the response does
/// not reveal which accounts exist.
/// </summary>
public sealed class LoginUserCommandHandler
    : ICommandHandler<LoginUserCommand, Result<UserDto>>
{
    private static readonly Error InvalidCredentials =
        Error.Unauthorized("Invalid email or password.");

    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> HandleAsync(
        LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var email = (command.Email ?? string.Empty).Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email, cancellationToken);
        if (user is null)
            return InvalidCredentials;

        return _passwordHasher.Verify(command.Password ?? string.Empty, user.PasswordHash)
            ? user.ToDto()
            : InvalidCredentials;
    }
}
