using Microsoft.Extensions.DependencyInjection;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Infrastructure.Messaging;

/// <summary>
/// Publishes domain events to every <see cref="IDomainEventHandler{TEvent}"/>
/// registered for the event's runtime type. Because handlers are discovered
/// purely through the DI container, a publisher (the Tasks module) stays unaware
/// of its subscribers (the Notifications module) — the loosely coupled,
/// event-driven backbone of the architecture. An event with no subscribers is a
/// no-op.
/// </summary>
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    public DomainEventDispatcher(IServiceProvider provider) => _provider = provider;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
            await DispatchAsync(domainEvent, cancellationToken);
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handle = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

        foreach (var handler in _provider.GetServices(handlerType))
        {
            if (handler is null) continue;
            await (Task)handle.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
        }
    }
}
