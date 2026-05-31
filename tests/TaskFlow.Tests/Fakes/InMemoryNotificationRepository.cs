using TaskFlow.Modules.Notifications.Domain;

namespace TaskFlow.Tests.Fakes;

public sealed class InMemoryNotificationRepository : InMemoryRepository<Notification>, INotificationRepository
{
    public Task<IReadOnlyList<Notification>> GetByRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default) =>
        Task.FromResult((IReadOnlyList<Notification>)Items.Where(n => n.RecipientId == recipientId).ToList());
}
