using TaskFlow.Modules.Projects.Domain.Events;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Domain;

/// <summary>
/// The Project aggregate root. Owns its membership list and guarantees the
/// project's invariants: every project has exactly one owner, a user can join
/// only once, the owner can never be removed, and an archived project is frozen.
/// All state changes go through behaviour methods that raise domain events, so
/// the aggregate can never be left in an invalid state.
/// </summary>
public sealed class Project : AggregateRoot
{
    private readonly List<ProjectMember> _members = new();

    private Project(Guid id, string name, string description, Guid ownerId, DateTime createdOnUtc)
        : base(id)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        Status = ProjectStatus.Active;
        CreatedOnUtc = createdOnUtc;
    }

    /// <summary>Parameterless constructor for serializers.</summary>
    private Project() { }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    /// <summary>The user who created the project and holds the <see cref="ProjectRole.Owner"/> membership.</summary>
    public Guid OwnerId { get; private set; }

    public ProjectStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

    public bool IsArchived => Status == ProjectStatus.Archived;

    /// <summary>
    /// Factory for a brand-new project. The owner is enrolled automatically as the
    /// sole <see cref="ProjectRole.Owner"/> member.
    /// </summary>
    public static Result<Project> Create(string name, Guid ownerId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("A project name is required.");
        if (ownerId == Guid.Empty)
            return Error.Validation("An owner is required.");

        var project = new Project(Guid.NewGuid(), name.Trim(), (description ?? string.Empty).Trim(), ownerId, DateTime.UtcNow);
        project._members.Add(new ProjectMember(Guid.NewGuid(), project.Id, ownerId, ProjectRole.Owner, project.CreatedOnUtc));
        project.Raise(new ProjectCreatedEvent(project.Id, project.Name, ownerId));
        return project;
    }

    /// <summary>Adds a user to the project. Ownership is set once at creation and cannot be granted here.</summary>
    public Result AddMember(Guid userId, ProjectRole role)
    {
        if (IsArchived)
            return Result.Failure(Error.Conflict("Cannot modify the membership of an archived project."));
        if (userId == Guid.Empty)
            return Result.Failure(Error.Validation("A user is required."));
        if (role == ProjectRole.Owner)
            return Result.Failure(Error.Validation("A project's owner is set at creation and cannot be reassigned."));
        if (_members.Any(member => member.UserId == userId))
            return Result.Failure(Error.Conflict("The user is already a member of this project."));

        _members.Add(new ProjectMember(Guid.NewGuid(), Id, userId, role, DateTime.UtcNow));
        Raise(new ProjectMemberAddedEvent(Id, userId, role));
        return Result.Success();
    }

    /// <summary>Removes a user from the project. The owner cannot be removed.</summary>
    public Result RemoveMember(Guid userId)
    {
        if (IsArchived)
            return Result.Failure(Error.Conflict("Cannot modify the membership of an archived project."));
        if (userId == OwnerId)
            return Result.Failure(Error.Validation("The project owner cannot be removed."));

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
            return Result.Failure(Error.NotFound("The user is not a member of this project."));

        _members.Remove(member);
        return Result.Success();
    }

    /// <summary>Archives the project, freezing its membership. Idempotency is treated as a conflict.</summary>
    public Result Archive()
    {
        if (IsArchived)
            return Result.Failure(Error.Conflict("The project is already archived."));

        Status = ProjectStatus.Archived;
        return Result.Success();
    }

    /// <summary>Renames the project and updates its description. An archived project is frozen.</summary>
    public Result UpdateDetails(string name, string? description)
    {
        if (IsArchived)
            return Result.Failure(Error.Conflict("Cannot edit an archived project."));
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("A project name is required."));

        Name = name.Trim();
        Description = (description ?? string.Empty).Trim();
        return Result.Success();
    }

    public bool HasMember(Guid userId) => _members.Any(member => member.UserId == userId);
}
