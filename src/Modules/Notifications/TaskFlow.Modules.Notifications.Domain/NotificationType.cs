namespace TaskFlow.Modules.Notifications.Domain;

/// <summary>
/// The kind of event a <see cref="Notification"/> was raised for. Mirrors the
/// cross-module task events the Notifications module subscribes to.
/// </summary>
public enum NotificationType
{
    TaskAssigned = 1,
    TaskCompleted = 2
}
