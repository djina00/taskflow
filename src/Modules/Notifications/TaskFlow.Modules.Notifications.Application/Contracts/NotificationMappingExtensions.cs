using TaskFlow.Modules.Notifications.Domain;

namespace TaskFlow.Modules.Notifications.Application.Contracts;

/// <summary>
/// Maps the <see cref="Notification"/> aggregate to its outward-facing
/// <see cref="NotificationDto"/>. Centralised here so every use case projects
/// notifications identically.
/// </summary>
internal static class NotificationMappingExtensions
{
    public static NotificationDto ToDto(this Notification notification) => new(
        notification.Id,
        notification.RecipientId,
        notification.Type.ToString(),
        notification.Message,
        notification.IsRead,
        notification.CreatedOnUtc);
}
