using TaskFlow.Modules.Users.Application.Abstractions;
using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.Modules.Users.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Commands.RegisterUser;

/// <summary>
/// Orchestrates registration: enforces application-level rules (password policy,
/// email uniqueness), hashes the password, then delegates the invariant checks
/// and state change to the <see cref="User"/> aggregate.
/// </summary>
public sealed class RegisterUserCommandHandler
    : ICommandHandler<RegisterUserCommand, Result<UserDto>>
{
    private const int MinimumPasswordLength = 6;

    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> HandleAsync(
        RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < MinimumPasswordLength)
            return Error.Validation($"Password must be at least {MinimumPasswordLength} characters.");

        var email = (command.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (await _users.ExistsByEmailAsync(email, cancellationToken))
            return Error.Conflict($"A user with email '{email}' already exists.");

        var passwordHash = _passwordHasher.Hash(command.Password);

        var result = User.Register(command.Email ?? string.Empty, passwordHash, command.FullName);
        if (result.IsFailure)
            return result.Error;

        await _users.AddAsync(result.Value, cancellationToken);
        return result.Value.ToDto();
    }
}
