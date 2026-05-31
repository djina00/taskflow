using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Modules.Notifications.Domain;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Notifications.Infrastructure;

/// <summary>
/// Persistence adapter for the <see cref="Notification"/> aggregate. Reuses the
/// generic <see cref="DocumentRepository{T}"/> for CRUD and adds the recipient
/// lookup the use cases need.
/// </summary>
public sealed class NotificationRepository : DocumentRepository<Notification>, INotificationRepository
{
    public NotificationRepository(IDocumentStore store, IDomainEventDispatcher dispatcher)
        : base(store, dispatcher) { }

    public async Task<IReadOnlyList<Notification>> GetByRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        return all.Where(notification => notification.RecipientId == recipientId).ToList();
    }
}
