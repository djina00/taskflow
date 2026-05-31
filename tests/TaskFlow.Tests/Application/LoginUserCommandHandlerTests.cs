using TaskFlow.Modules.Users.Application.Commands.LoginUser;
using TaskFlow.Modules.Users.Domain;
using TaskFlow.Tests.Fakes;

namespace TaskFlow.Tests.Application;

public class LoginUserCommandHandlerTests
{
    private readonly InMemoryUserRepository _users = new();
    private readonly FakePasswordHasher _hasher = new();

    public LoginUserCommandHandlerTests()
    {
        // Seed a known account whose stored hash matches the fake hasher.
        var user = User.Register("ada@example.com", _hasher.Hash("secret123"), "Ada Lovelace").Value;
        _users.AddAsync(user).GetAwaiter().GetResult();
    }

    private LoginUserCommandHandler CreateHandler() => new(_users, _hasher);

    [Fact]
    public async Task Succeeds_with_the_correct_password()
    {
        var result = await CreateHandler().HandleAsync(new LoginUserCommand("ada@example.com", "secret123"));

        Assert.True(result.IsSuccess);
        Assert.Equal("ada@example.com", result.Value.Email);
    }

    [Fact]
    public async Task Fails_with_a_wrong_password()
    {
        var result = await CreateHandler().HandleAsync(new LoginUserCommand("ada@example.com", "wrong"));

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Fails_with_an_unknown_email_using_the_same_opaque_error()
    {
        var result = await CreateHandler().HandleAsync(new LoginUserCommand("nobody@example.com", "secret123"));

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Unauthorized", result.Error.Code);
    }
}
