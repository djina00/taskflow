using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Projects.Domain;

/// <summary>
/// A user's membership in a project. Part of the <see cref="Project"/> aggregate
/// — it is created, mutated and removed only through the aggregate root, never
/// independently. Carries its own identity (inherited from <see cref="BaseEntity"/>)
/// so the relational/JSON persistence adapters can address each row, while the
/// back-reference to <see cref="ProjectId"/> records which project it belongs to.
/// </summary>
public sealed class ProjectMember : BaseEntity
{
    internal ProjectMember(Guid id, Guid projectId, Guid userId, ProjectRole role, DateTime joinedOnUtc)
        : base(id)
    {
        ProjectId = projectId;
        UserId = userId;
        Role = role;
        JoinedOnUtc = joinedOnUtc;
    }

    /// <summary>Parameterless constructor for serializers.</summary>
    private ProjectMember() { }

    public Guid ProjectId { get; private set; }

    public Guid UserId { get; private set; }

    public ProjectRole Role { get; private set; }

    public DateTime JoinedOnUtc { get; private set; }
}
