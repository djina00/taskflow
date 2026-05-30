namespace TaskFlow.SharedKernel.Domain.Events;

/// <summary>
/// Raised by the Tasks module when a task is completed. Defined in the
/// SharedKernel so subscribers in other modules (e.g. Notifications, Reports)
/// can react without referencing the Tasks module. Carries the (optional)
/// assignee so a subscriber can notify the right person.
/// </summary>
public sealed record TaskCompletedEvent(Guid TaskId, Guid? AssigneeId, string Title) : DomainEvent;
