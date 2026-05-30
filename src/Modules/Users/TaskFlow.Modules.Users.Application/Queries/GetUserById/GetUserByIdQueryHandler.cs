using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.Modules.Users.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler
    : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _users;

    public GetUserByIdQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<UserDto>> HandleAsync(
        GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(query.UserId, cancellationToken);
        return user is null
            ? Error.NotFound($"User '{query.UserId}' was not found.")
            : user.ToDto();
    }
}
