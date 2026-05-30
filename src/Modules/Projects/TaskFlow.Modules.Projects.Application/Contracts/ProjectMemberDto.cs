namespace TaskFlow.Modules.Projects.Application.Contracts;

/// <summary>
/// Read model for a single project membership. The project role is projected as
/// its name so consumers (UI, reports) never depend on the domain enumeration.
/// </summary>
public sealed record ProjectMemberDto(
    Guid UserId,
    string Role,
    DateTime JoinedOnUtc);
