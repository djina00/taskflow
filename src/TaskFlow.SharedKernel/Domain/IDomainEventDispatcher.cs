namespace TaskFlow.SharedKernel.Domain;

/// <summary>
/// Port for publishing domain events to every registered handler. The concrete
/// adapter (in the Infrastructure layer) resolves handlers from the DI
/// container, keeping publishers unaware of who is subscribing.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
