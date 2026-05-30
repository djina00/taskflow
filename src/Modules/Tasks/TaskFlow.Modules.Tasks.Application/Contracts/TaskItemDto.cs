namespace TaskFlow.Modules.Tasks.Application.Contracts;

/// <summary>
/// Read model exposed by the Tasks use cases to the outside world (UI, reports).
/// Projects the aggregate's enums as their names and its comments as DTOs so the
/// domain entities are never leaked across the application boundary.
/// </summary>
public sealed record TaskItemDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Description,
    string Priority,
    string Status,
    Guid? AssigneeId,
    DateTime CreatedOnUtc,
    DateTime? CompletedOnUtc,
    IReadOnlyCollection<CommentDto> Comments);
