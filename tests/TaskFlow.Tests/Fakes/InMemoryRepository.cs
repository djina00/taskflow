using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Tests.Fakes;

/// <summary>
/// A tiny in-memory <see cref="IRepository{T}"/> used to unit-test the use cases in
/// isolation. Substituting this fake for the real persistence adapter is exactly the
/// point of the repository port: the application layer neither knows nor cares that
/// the store is now a <see cref="List{T}"/> instead of JSON or SQLite.
/// </summary>
public abstract class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly List<T> Items = new();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(entity => entity.Id == id));

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult((IReadOnlyList<T>)Items.ToList());

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var index = Items.FindIndex(existing => existing.Id == entity.Id);
        if (index >= 0)
            Items[index] = entity;
        else
            Items.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Items.RemoveAll(entity => entity.Id == id);
        return Task.CompletedTask;
    }
}
