using TaskFlow.Modules.Users.Application.Commands.RegisterUser;
using TaskFlow.Tests.Fakes;

namespace TaskFlow.Tests.Application;

public class RegisterUserCommandHandlerTests
{
    private readonly InMemoryUserRepository _users = new();
    private readonly FakePasswordHasher _hasher = new();

    private RegisterUserCommandHandler CreateHandler() => new(_users, _hasher);

    [Fact]
    public async Task Registers_a_new_user_and_stores_only_the_hash()
    {
        var result = await CreateHandler().HandleAsync(
            new RegisterUserCommand("ada@example.com", "secret123", "Ada Lovelace"));

        Assert.True(result.IsSuccess);
        var stored = await _users.GetByEmailAsync("ada@example.com");
        Assert.NotNull(stored);
        Assert.Equal("hashed:secret123", stored!.PasswordHash);   // never the plaintext
    }

    [Fact]
    public async Task Rejects_a_duplicate_email()
    {
        await CreateHandler().HandleAsync(new RegisterUserCommand("ada@example.com", "secret123", "Ada"));

        var result = await CreateHandler().HandleAsync(new RegisterUserCommand("ADA@example.com", "another1", "Ada II"));

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Conflict", result.Error.Code);
    }

    [Fact]
    public async Task Rejects_a_password_below_the_minimum_length()
    {
        var result = await CreateHandler().HandleAsync(new RegisterUserCommand("ada@example.com", "short", "Ada"));

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Validation", result.Error.Code);
        Assert.Empty(await _users.GetAllAsync());
    }
}
