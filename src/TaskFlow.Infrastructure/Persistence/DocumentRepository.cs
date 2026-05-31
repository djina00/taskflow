using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>
/// Generic <see cref="IRepository{T}"/> implementation over an
/// <see cref="IDocumentStore"/>. Written once and reused by every module's
/// repository, it is completely unaware of whether the underlying store is a JSON
/// file or a SQLite database. After every write it publishes the aggregate's
/// pending domain events through the <see cref="IDomainEventDispatcher"/> — the
/// hook that makes the event-driven flow real, e.g. a <c>TaskAssignedEvent</c>
/// reaching the Notifications module once the task has been saved.
/// </summary>
public abstract class DocumentRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IDocumentStore _store;
    private readonly IDomainEventDispatcher _dispatcher;

    protected DocumentRepository(IDocumentStore store, IDomainEventDispatcher dispatcher)
    {
        _store = store;
        _dispatcher = dispatcher;
    }

    /// <summary>The store collection name — the aggregate's type name by default.</summary>
    protected virtual string Collection => typeof(T).Name;

    /// <summary>Loads the whole collection. The seam derived repositories filter over for their custom lookups.</summary>
    protected Task<List<T>> LoadAsync(CancellationToken cancellationToken) =>
        _store.ReadAllAsync<T>(Collection, cancellationToken);

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        return all.FirstOrDefault(entity => entity.Id == id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await LoadAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        all.Add(entity);
        await _store.WriteAllAsync(Collection, all, cancellationToken);
        await DispatchEventsAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        var index = all.FindIndex(existing => existing.Id == entity.Id);
        if (index >= 0)
            all[index] = entity;
        else
            all.Add(entity);

        await _store.WriteAllAsync(Collection, all, cancellationToken);
        await DispatchEventsAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        if (all.RemoveAll(entity => entity.Id == id) > 0)
            await _store.WriteAllAsync(Collection, all, cancellationToken);
    }

    private async Task DispatchEventsAsync(T entity, CancellationToken cancellationToken)
    {
        if (entity is not AggregateRoot aggregate || aggregate.DomainEvents.Count == 0)
            return;

        var events = aggregate.DomainEvents.ToArray();
        aggregate.ClearDomainEvents();
        await _dispatcher.DispatchAsync(events, cancellationToken);
    }
}
