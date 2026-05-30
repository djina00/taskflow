using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Projects.Domain.Events;

/// <summary>
/// Raised when a new project is created. Kept inside the Projects module while
/// only the module itself reacts to it; it can be promoted to the SharedKernel
/// later if another module (e.g. Notifications) needs to react.
/// </summary>
public sealed record ProjectCreatedEvent(Guid ProjectId, string Name, Guid OwnerId) : DomainEvent;
