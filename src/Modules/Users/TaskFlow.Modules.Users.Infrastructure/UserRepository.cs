using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Modules.Users.Domain;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Users.Infrastructure;

/// <summary>
/// Persistence adapter for the <see cref="User"/> aggregate. The generic
/// <see cref="DocumentRepository{T}"/> base provides the standard CRUD and the
/// store-agnostic loading; this type only adds the email lookups the use cases
/// need, filtering the loaded set in memory.
/// </summary>
public sealed class UserRepository : DocumentRepository<User>, IUserRepository
{
    public UserRepository(IDocumentStore store, IDomainEventDispatcher dispatcher)
        : base(store, dispatcher) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = (email ?? string.Empty).Trim();
        var all = await LoadAsync(cancellationToken);
        return all.FirstOrDefault(user =>
            string.Equals(user.Email, normalized, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await GetByEmailAsync(email, cancellationToken) is not null;
}
