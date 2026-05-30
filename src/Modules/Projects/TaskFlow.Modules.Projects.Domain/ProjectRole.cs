namespace TaskFlow.Modules.Projects.Domain;

/// <summary>
/// A member's level of involvement within a single project. Unlike the
/// system-wide <c>Role</c> in the Users module (a catalogue entity), project
/// membership is a small, fixed, project-scoped concept and is best modelled as
/// an enumeration owned by the <see cref="Project"/> aggregate.
/// </summary>
public enum ProjectRole
{
    /// <summary>The creator of the project. Exactly one per project; cannot be removed.</summary>
    Owner = 1,

    /// <summary>Can contribute to the project's work.</summary>
    Collaborator = 2,

    /// <summary>Read-only participant.</summary>
    Viewer = 3
}
