using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Users.Application.Queries.GetAllUsers;

/// <summary>Lists all users as read models.</summary>
public sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>;
