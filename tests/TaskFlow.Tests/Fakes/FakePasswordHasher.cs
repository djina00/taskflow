using TaskFlow.Modules.Users.Application.Abstractions;

namespace TaskFlow.Tests.Fakes;

/// <summary>
/// A deterministic, non-cryptographic <see cref="IPasswordHasher"/> for tests. It
/// makes the hashing seam trivial to reason about while still exercising the
/// hash/verify contract the use cases depend on.
/// </summary>
public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string passwordHash) => passwordHash == Hash(password);
}
