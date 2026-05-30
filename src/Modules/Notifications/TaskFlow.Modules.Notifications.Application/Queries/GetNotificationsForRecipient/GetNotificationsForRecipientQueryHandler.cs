using TaskFlow.Modules.Notifications.Application.Contracts;
using TaskFlow.Modules.Notifications.Domain;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Notifications.Application.Queries.GetNotificationsForRecipient;

public sealed class GetNotificationsForRecipientQueryHandler
    : IQueryHandler<GetNotificationsForRecipientQuery, IReadOnlyList<NotificationDto>>
{
    private readonly INotificationRepository _notifications;

    public GetNotificationsForRecipientQueryHandler(INotificationRepository notifications)
        => _notifications = notifications;

    public async Task<IReadOnlyList<NotificationDto>> HandleAsync(
        GetNotificationsForRecipientQuery query, CancellationToken cancellationToken = default)
    {
        var notifications = await _notifications.GetByRecipientAsync(query.RecipientId, cancellationToken);
        return notifications.Select(notification => notification.ToDto()).ToArray();
    }
}
