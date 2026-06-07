using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Domain.Events;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Domain;

/// <summary>
/// The TaskItem aggregate root (named to avoid clashing with
/// <see cref="System.Threading.Tasks.Task"/>). Owns its comments and guards the
/// task workflow: a task moves Todo → InProgress → Done, can be (re)assigned only
/// while it is not yet completed, and raises the cross-module
/// <see cref="TaskAssignedEvent"/> / <see cref="TaskCompletedEvent"/> so other
/// modules can react without referencing Tasks. The owning project is referenced
/// by id only — the Tasks module never takes a dependency on the Projects module.
/// </summary>
public sealed class TaskItem : AggregateRoot
{
    private readonly List<Comment> _comments = new();

    private TaskItem(Guid id, Guid projectId, string title, string description, TaskPriority priority, Guid reporterId, DateTime createdOnUtc)
        : base(id)
    {
        ProjectId = projectId;
        Title = title;
        Description = description;
        Priority = priority;
        ReporterId = reporterId;
        Status = TaskItemStatus.Todo;
        CreatedOnUtc = createdOnUtc;
    }

    /// <summary>Parameterless constructor for serializers.</summary>
    private TaskItem() { }

    public Guid ProjectId { get; private set; }

    /// <summary>The user who created (reported) the task.</summary>
    public Guid ReporterId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public TaskPriority Priority { get; private set; }

    public TaskItemStatus Status { get; private set; }

    /// <summary>The user the task is assigned to, or <c>null</c> while unassigned.</summary>
    public Guid? AssigneeId { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>When the task was completed, or <c>null</c> while it is still open.</summary>
    public DateTime? CompletedOnUtc { get; private set; }

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public bool IsCompleted => Status == TaskItemStatus.Done;

    /// <summary>
    /// Factory for a brand-new task. The task starts unassigned and in the Todo state;
    /// the reporter is the user who created it.
    /// </summary>
    public static Result<TaskItem> Create(Guid projectId, string title, string? description, TaskPriority priority, Guid reporterId)
    {
        if (projectId == Guid.Empty)
            return Error.Validation("A project is required.");
        if (string.IsNullOrWhiteSpace(title))
            return Error.Validation("A task title is required.");
        if (reporterId == Guid.Empty)
            return Error.Validation("A reporter is required.");

        return new TaskItem(Guid.NewGuid(), projectId, title.Trim(), (description ?? string.Empty).Trim(), priority, reporterId, DateTime.UtcNow);
    }

    /// <summary>Assigns (or reassigns) the task to a user. A completed task cannot be reassigned.</summary>
    public Result Assign(Guid assigneeId)
    {
        if (assigneeId == Guid.Empty)
            return Result.Failure(Error.Validation("An assignee is required."));
        if (IsCompleted)
            return Result.Failure(Error.Conflict("A completed task cannot be reassigned."));
        if (AssigneeId == assigneeId)
            return Result.Failure(Error.Conflict("The task is already assigned to this user."));

        AssigneeId = assigneeId;
        Raise(new TaskAssignedEvent(Id, assigneeId, Title));
        return Result.Success();
    }

    /// <summary>Moves the task into progress. Only a Todo task can be started.</summary>
    public Result Start()
    {
        if (Status != TaskItemStatus.Todo)
            return Result.Failure(Error.Conflict("Only a task in the Todo state can be started."));

        Status = TaskItemStatus.InProgress;
        return Result.Success();
    }

    /// <summary>Marks the task complete. Idempotency is treated as a conflict.</summary>
    public Result Complete()
    {
        if (IsCompleted)
            return Result.Failure(Error.Conflict("The task is already completed."));

        Status = TaskItemStatus.Done;
        CompletedOnUtc = DateTime.UtcNow;
        Raise(new TaskCompletedEvent(Id, AssigneeId, Title));
        return Result.Success();
    }

    /// <summary>Updates the task's title, description and priority.</summary>
    public Result UpdateDetails(string title, string? description, TaskPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation("A task title is required."));

        Title = title.Trim();
        Description = (description ?? string.Empty).Trim();
        Priority = priority;
        return Result.Success();
    }

    /// <summary>Adds a comment to the task.</summary>
    public Result AddComment(Guid authorId, string text)
    {
        if (authorId == Guid.Empty)
            return Result.Failure(Error.Validation("A comment author is required."));
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure(Error.Validation("Comment text is required."));

        _comments.Add(new Comment(Guid.NewGuid(), Id, authorId, text.Trim(), DateTime.UtcNow));
        return Result.Success();
    }
}
