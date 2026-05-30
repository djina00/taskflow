using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Users.Domain.Events;

/// <summary>
/// Raised when a new user successfully registers. Kept inside the Users module
/// while only the module itself reacts to it; it can be promoted to the
/// SharedKernel later if another module (e.g. Notifications) needs to react.
/// </summary>
public sealed record UserRegisteredEvent(Guid UserId, string Email) : DomainEvent;
