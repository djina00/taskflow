using TaskFlow.Modules.Users.Domain;
using TaskFlow.Modules.Users.Domain.Events;

namespace TaskFlow.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Register_with_valid_data_normalizes_email_defaults_to_member_and_raises_event()
    {
        var result = User.Register("  Ada@Example.COM ", "hashed", "Ada Lovelace");

        Assert.True(result.IsSuccess);
        var user = result.Value;
        Assert.Equal("ada@example.com", user.Email);          // trimmed + lower-cased
        Assert.True(user.IsInRole(Role.Member));
        Assert.Contains(user.DomainEvents, e => e is UserRegisteredEvent);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    [InlineData("@nope.com")]
    public void Register_with_invalid_email_fails(string email)
    {
        var result = User.Register(email, "hashed", "Name");

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Validation", result.Error.Code);
    }

    [Fact]
    public void Register_without_password_hash_fails()
    {
        var result = User.Register("ada@example.com", "   ", "Ada");

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void AssignRole_twice_is_a_conflict()
    {
        var user = User.Register("ada@example.com", "hashed", "Ada").Value;

        var first = user.AssignRole(Role.Admin);
        var second = user.AssignRole(Role.Admin);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsFailure);
        Assert.Equal("Error.Conflict", second.Error.Code);
    }

    [Fact]
    public void RemoveRole_cannot_drop_the_last_remaining_role()
    {
        var user = User.Register("ada@example.com", "hashed", "Ada").Value;

        // The user starts with only Member; removing it must be rejected.
        var result = user.RemoveRole(Role.Member);

        Assert.True(result.IsFailure);
        Assert.True(user.IsInRole(Role.Member));
    }
}
