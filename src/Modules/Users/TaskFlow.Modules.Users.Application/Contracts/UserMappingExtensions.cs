using TaskFlow.Modules.Users.Domain;

namespace TaskFlow.Modules.Users.Application.Contracts;

/// <summary>
/// Maps the <see cref="User"/> aggregate to its outward-facing <see cref="UserDto"/>.
/// Centralised here so every use case projects users identically.
/// </summary>
internal static class UserMappingExtensions
{
    public static UserDto ToDto(this User user) => new(
        user.Id,
        user.Email,
        user.FullName,
        user.Roles.Select(role => role.Name).ToArray(),
        user.CreatedOnUtc);
}
