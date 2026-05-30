using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Commands.RemoveProjectMember;

/// <summary>Removes a user from a project. The owner cannot be removed.</summary>
public sealed record RemoveProjectMemberCommand(Guid ProjectId, Guid UserId) : ICommand<Result>;
