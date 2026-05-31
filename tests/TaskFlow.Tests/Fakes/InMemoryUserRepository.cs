using TaskFlow.Modules.Users.Domain;

namespace TaskFlow.Tests.Fakes;

public sealed class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(user =>
            string.Equals(user.Email, (email ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)));

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await GetByEmailAsync(email, cancellationToken) is not null;
}
