using TaskFlow.Modules.Tasks.Domain;

namespace TaskFlow.Modules.Tasks.Application.Contracts;

/// <summary>
/// Maps the <see cref="TaskItem"/> aggregate to its outward-facing
/// <see cref="TaskItemDto"/>. Centralised here so every use case projects tasks
/// identically.
/// </summary>
internal static class TaskMappingExtensions
{
    public static TaskItemDto ToDto(this TaskItem task) => new(
        task.Id,
        task.ProjectId,
        task.Title,
        task.Description,
        task.Priority.ToString(),
        task.Status.ToString(),
        task.ReporterId,
        task.AssigneeId,
        task.CreatedOnUtc,
        task.CompletedOnUtc,
        task.Comments.Select(comment => comment.ToDto()).ToArray());

    private static CommentDto ToDto(this Comment comment) => new(
        comment.AuthorId,
        comment.Text,
        comment.CreatedOnUtc);
}
