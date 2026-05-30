using TaskFlow.Modules.Users.Domain.Events;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Domain;

/// <summary>
/// The User aggregate root. Encapsulates identity, credentials (stored only as a
/// hash — never plaintext) and the set of roles granted to the account. All state
/// transitions go through behaviour methods that enforce the invariants and raise
/// domain events, so the aggregate can never be left in an invalid state.
/// </summary>
public sealed class User : AggregateRoot
{
    private readonly List<Role> _roles = new();

    private User(Guid id, string email, string passwordHash, string fullName, DateTime createdOnUtc)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        CreatedOnUtc = createdOnUtc;
    }

    /// <summary>Parameterless constructor for serializers.</summary>
    private User() { }

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public DateTime CreatedOnUtc { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Factory for a brand-new user. Accepts an already-hashed password so the
    /// domain never sees the plaintext. Every user starts as a <see cref="Role.Member"/>.
    /// </summary>
    public static Result<User> Register(string email, string passwordHash, string fullName)
    {
        var normalizedEmail = (email ?? string.Empty).Trim().ToLowerInvariant();

        if (!IsValidEmail(normalizedEmail))
            return Error.Validation("A valid email address is required.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Error.Validation("A password hash is required.");
        if (string.IsNullOrWhiteSpace(fullName))
            return Error.Validation("A full name is required.");

        var user = new User(Guid.NewGuid(), normalizedEmail, passwordHash, fullName.Trim(), DateTime.UtcNow);
        user._roles.Add(Role.Member);
        user.Raise(new UserRegisteredEvent(user.Id, user.Email));
        return user;
    }

    /// <summary>Grants a role to the user. Idempotency is treated as a conflict.</summary>
    public Result AssignRole(Role role)
    {
        if (role is null)
            return Result.Failure(Error.Validation("A role is required."));
        if (_roles.Contains(role))
            return Result.Failure(Error.Conflict($"User already has the '{role.Name}' role."));

        _roles.Add(role);
        Raise(new RoleAssignedEvent(Id, role.Name));
        return Result.Success();
    }

    /// <summary>Revokes a role. A user must always retain at least one role.</summary>
    public Result RemoveRole(Role role)
    {
        if (role is null)
            return Result.Failure(Error.Validation("A role is required."));
        if (!_roles.Contains(role))
            return Result.Failure(Error.NotFound($"User does not have the '{role.Name}' role."));
        if (_roles.Count == 1)
            return Result.Failure(Error.Validation("A user must retain at least one role."));

        _roles.Remove(role);
        return Result.Success();
    }

    public bool IsInRole(Role role) => role is not null && _roles.Contains(role);

    // A deliberately small, dependency-free sanity check — full RFC-compliant
    // validation is out of scope for this demo. Requires a '@' with at least one
    // character on each side and a '.' somewhere after the '@'.
    private static bool IsValidEmail(string email)
    {
        var at = email.IndexOf('@');
        if (at <= 0 || at == email.Length - 1)
            return false;

        var lastDot = email.LastIndexOf('.');
        return lastDot > at && lastDot < email.Length - 1;
    }
}
