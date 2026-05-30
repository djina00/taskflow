using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.AddProjectMember;

/// <summary>Adds a user to a project with the named project role (Collaborator or Viewer).</summary>
public sealed record AddProjectMemberCommand(Guid ProjectId, Guid UserId, string Role) : ICommand<Result>;
