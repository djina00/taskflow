namespace TaskFlow.SharedKernel.Domain;

/// <summary>
/// Generic persistence port (Repository Pattern). The Application layer depends
/// only on this abstraction; concrete adapters such as the SQLite or JSON
/// implementations are supplied via Dependency Injection. This is the seam
/// that keeps business logic independent of data-access technology.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
