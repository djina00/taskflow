using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Notifications.Application.Commands.MarkNotificationAsRead;

/// <summary>Marks a single notification as read.</summary>
public sealed record MarkNotificationAsReadCommand(Guid NotificationId) : ICommand<Result>;
