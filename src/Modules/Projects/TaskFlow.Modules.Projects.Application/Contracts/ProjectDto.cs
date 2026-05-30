namespace TaskFlow.Modules.Projects.Application.Contracts;

/// <summary>
/// Read model exposed by the Projects use cases to the outside world (UI,
/// reports). Projects the aggregate's membership as DTOs so the domain entities
/// are never leaked across the application boundary.
/// </summary>
public sealed record ProjectDto(
    Guid Id,
    string Name,
    string Description,
    Guid OwnerId,
    string Status,
    DateTime CreatedOnUtc,
    IReadOnlyCollection<ProjectMemberDto> Members);
