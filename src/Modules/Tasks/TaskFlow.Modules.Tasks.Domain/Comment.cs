using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Tasks.Domain;

/// <summary>
/// A comment left on a task. Part of the <see cref="TaskItem"/> aggregate — it is
/// created only through the aggregate root, never independently. Carries its own
/// identity (inherited from <see cref="BaseEntity"/>) plus a back-reference to the
/// owning <see cref="TaskId"/> so the persistence adapters can address each row.
/// </summary>
public sealed class Comment : BaseEntity
{
    internal Comment(Guid id, Guid taskId, Guid authorId, string text, DateTime createdOnUtc)
        : base(id)
    {
        TaskId = taskId;
        AuthorId = authorId;
        Text = text;
        CreatedOnUtc = createdOnUtc;
    }

    /// <summary>Parameterless constructor for serializers.</summary>
    private Comment() { }

    public Guid TaskId { get; private set; }

    public Guid AuthorId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public DateTime CreatedOnUtc { get; private set; }
}
