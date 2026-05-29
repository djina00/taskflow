namespace TaskFlow.SharedKernel.Domain;

/// <summary>
/// Marker for something that happened in the domain that other parts of the
/// system may care about. Domain events are the backbone of the loosely
/// coupled, event-driven communication between modules.
/// </summary>
public interface IDomainEvent
{
    /// <summary>When the event occurred (UTC).</summary>
    DateTime OccurredOnUtc { get; }
}

/// <summary>
/// Convenience base record for domain events. Records give value-based
/// equality and concise declarations for the event payloads.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
