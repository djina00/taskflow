using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Projects.Domain.Events;

/// <summary>
/// Raised when a user is added to a project. Captures the membership change
/// without exposing the full aggregate.
/// </summary>
public sealed record ProjectMemberAddedEvent(Guid ProjectId, Guid UserId, ProjectRole Role) : DomainEvent;
