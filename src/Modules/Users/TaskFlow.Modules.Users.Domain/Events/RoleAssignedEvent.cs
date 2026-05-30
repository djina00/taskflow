using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Users.Domain.Events;

/// <summary>
/// Raised when a role is granted to a user. Captures the audit-worthy fact that
/// an authorization change occurred without exposing the full aggregate.
/// </summary>
public sealed record RoleAssignedEvent(Guid UserId, string RoleName) : DomainEvent;
