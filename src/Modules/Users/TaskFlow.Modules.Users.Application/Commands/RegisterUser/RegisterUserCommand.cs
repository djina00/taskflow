using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Application.Commands.RegisterUser;

/// <summary>Registers a new user account from a plaintext password.</summary>
public sealed record RegisterUserCommand(string Email, string Password, string FullName)
    : ICommand<Result<UserDto>>;
