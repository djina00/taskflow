namespace TaskFlow.Modules.Projects.Domain;

/// <summary>
/// The lifecycle state of a <see cref="Project"/>. An archived project is
/// read-only: its membership can no longer change.
/// </summary>
public enum ProjectStatus
{
    Active = 1,
    Archived = 2
}
