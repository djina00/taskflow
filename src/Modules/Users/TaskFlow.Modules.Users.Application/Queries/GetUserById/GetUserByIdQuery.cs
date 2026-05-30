using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Queries.GetUserById;

/// <summary>Fetches a single user by identifier.</summary>
public sealed record GetUserByIdQuery(Guid UserId) : IQuery<Result<UserDto>>;
