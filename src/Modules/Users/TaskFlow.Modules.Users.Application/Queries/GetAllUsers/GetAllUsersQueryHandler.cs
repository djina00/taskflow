using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.Modules.Users.Domain;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Users.Application.Queries.GetAllUsers;

public sealed class GetAllUsersQueryHandler
    : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _users;

    public GetAllUsersQueryHandler(IUserRepository users) => _users = users;

    public async Task<IReadOnlyList<UserDto>> HandleAsync(
        GetAllUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = await _users.GetAllAsync(cancellationToken);
        return users.Select(user => user.ToDto()).ToArray();
    }
}
