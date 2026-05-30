using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Users.Domain;

/// <summary>
/// Persistence port for the <see cref="User"/> aggregate. Extends the generic
/// <see cref="IRepository{T}"/> with the lookups the Users use cases need —
/// notably resolving a user by their (unique) email address. Concrete adapters
/// live in the Infrastructure layer and are supplied via Dependency Injection.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
