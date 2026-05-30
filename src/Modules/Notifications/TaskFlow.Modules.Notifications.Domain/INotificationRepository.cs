using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Notifications.Domain;

/// <summary>
/// Persistence port for the <see cref="Notification"/> aggregate. Extends the
/// generic <see cref="IRepository{T}"/> with the lookup the Notifications use
/// cases need — listing the notifications addressed to a recipient. Concrete
/// adapters live in the Infrastructure layer and are supplied via DI.
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    Task<IReadOnlyList<Notification>> GetByRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default);
}
