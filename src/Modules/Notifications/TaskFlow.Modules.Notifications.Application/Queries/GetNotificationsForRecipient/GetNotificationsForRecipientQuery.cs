using TaskFlow.Modules.Notifications.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Notifications.Application.Queries.GetNotificationsForRecipient;

/// <summary>Lists the notifications addressed to a recipient as read models.</summary>
public sealed record GetNotificationsForRecipientQuery(Guid RecipientId) : IQuery<IReadOnlyList<NotificationDto>>;
