using System.Text.Json.Serialization;

namespace TaskFlow.SharedKernel.Domain;

/// <summary>
/// An aggregate root is an entity that is the entry point to a cluster of
/// domain objects and is responsible for raising domain events. Only aggregate
/// roots may be loaded/saved through a repository.
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected AggregateRoot(Guid id) : base(id) { }

    protected AggregateRoot() { }

    /// <summary>
    /// Events raised by this aggregate that have not yet been dispatched.
    /// Ignored by serializers — events are transient, never persisted.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
