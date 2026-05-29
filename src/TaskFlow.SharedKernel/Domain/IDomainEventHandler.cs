namespace TaskFlow.SharedKernel.Domain;

/// <summary>
/// Handles a specific kind of domain event. Modules react to events from
/// other modules by implementing this interface — e.g. the Notifications
/// module handles a <c>TaskAssignedEvent</c> raised by the Tasks module,
/// without either module referencing the other.
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
