namespace TaskFlow.Modules.Tasks.Application.Contracts;

/// <summary>Read model for a single comment on a task.</summary>
public sealed record CommentDto(
    Guid AuthorId,
    string Text,
    DateTime CreatedOnUtc);
