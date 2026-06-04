namespace TaskFlow.SharedKernel.Domain.Events;

/// <summary>
/// Raised by the Notifications module the moment a notification is created. Defined
/// here in the SharedKernel — like the cross-module task events — so subscribers can
/// react without referencing the Notifications module. The presentation layer listens
/// to this so the UI can update the recipient's notification list live, instead of
/// waiting for a manual refresh.
/// </summary>
public sealed record NotificationCreatedEvent(Guid NotificationId, Guid RecipientId) : DomainEvent;
