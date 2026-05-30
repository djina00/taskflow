namespace TaskFlow.SharedKernel.Domain.Events;

/// <summary>
/// Raised by the Tasks module when a task is assigned to a user. Defined here in
/// the SharedKernel — rather than inside the Tasks module — so other modules
/// (e.g. Notifications) can subscribe to it without taking a reference on Tasks.
/// This is the contract that keeps cross-module communication loosely coupled.
/// </summary>
public sealed record TaskAssignedEvent(Guid TaskId, Guid AssigneeId, string Title) : DomainEvent;
