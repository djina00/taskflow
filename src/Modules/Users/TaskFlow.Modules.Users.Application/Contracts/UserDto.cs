namespace TaskFlow.Modules.Users.Application.Contracts;

/// <summary>
/// Read model exposed by the Users use cases to the outside world (UI, reports).
/// Deliberately omits the password hash and any other sensitive internals — the
/// aggregate is never leaked across the application boundary.
/// </summary>
public sealed record UserDto(
    Guid Id,
    string Email,
    string FullName,
    IReadOnlyCollection<string> Roles,
    DateTime CreatedOnUtc);
