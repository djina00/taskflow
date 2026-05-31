using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Users.Application.Abstractions;
using TaskFlow.Modules.Users.Domain;

namespace TaskFlow.Modules.Users.Infrastructure;

/// <summary>
/// Composition entry point for the Users module's infrastructure layer. Binds the
/// domain ports — the repository and the password hasher — to their concrete
/// adapters so the use cases registered by <c>AddUsersModule</c> can be resolved.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        return services;
    }
}
