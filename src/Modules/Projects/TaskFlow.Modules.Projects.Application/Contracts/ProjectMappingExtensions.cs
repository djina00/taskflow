using TaskFlow.Modules.Projects.Domain;

namespace TaskFlow.Modules.Projects.Application.Contracts;

/// <summary>
/// Maps the <see cref="Project"/> aggregate to its outward-facing
/// <see cref="ProjectDto"/>. Centralised here so every use case projects
/// projects identically.
/// </summary>
internal static class ProjectMappingExtensions
{
    public static ProjectDto ToDto(this Project project) => new(
        project.Id,
        project.Name,
        project.Description,
        project.OwnerId,
        project.Status.ToString(),
        project.CreatedOnUtc,
        project.Members.Select(member => member.ToDto()).ToArray());

    private static ProjectMemberDto ToDto(this ProjectMember member) => new(
        member.UserId,
        member.Role.ToString(),
        member.JoinedOnUtc);
}
