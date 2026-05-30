using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Users.Application.Commands.AssignRole;
using TaskFlow.Modules.Users.Application.Commands.LoginUser;
using TaskFlow.Modules.Users.Application.Commands.RegisterUser;
using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.Modules.Users.Application.Queries.GetAllUsers;
using TaskFlow.Modules.Users.Application.Queries.GetUserById;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application;

/// <summary>
/// Composition entry point for the Users module's application layer. Registers
/// the command and query handlers against their closed-generic interfaces so the
/// dispatchers can resolve them. Infrastructure adapters (repository,
/// password hasher) are registered separately by the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterUserCommand, Result<UserDto>>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginUserCommand, Result<UserDto>>, LoginUserCommandHandler>();
        services.AddScoped<ICommandHandler<AssignRoleCommand, Result>, AssignRoleCommandHandler>();

        services.AddScoped<IQueryHandler<GetUserByIdQuery, Result<UserDto>>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>, GetAllUsersQueryHandler>();

        return services;
    }
}
