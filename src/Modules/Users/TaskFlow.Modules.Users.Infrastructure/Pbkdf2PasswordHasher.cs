using System.Security.Cryptography;
using TaskFlow.Modules.Users.Application.Abstractions;

namespace TaskFlow.Modules.Users.Infrastructure;

/// <summary>
/// PBKDF2 (SHA-256) implementation of the <see cref="IPasswordHasher"/> port. Each
/// password gets a fresh random salt; the iteration count, salt and derived key are
/// packed into a single self-describing string so verification needs no external
/// metadata. Verification is constant-time to avoid leaking information through
/// timing. Living in the Infrastructure layer keeps the cryptography out of the
/// domain and use cases entirely.
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;   // 128-bit salt
    private const int KeySize = 32;    // 256-bit derived key
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return string.Join('.', Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    public bool Verify(string password, string passwordHash)
    {
        var segments = passwordHash.Split('.', 3);
        if (segments.Length != 3 || !int.TryParse(segments[0], out var iterations))
            return false;

        byte[] salt, expectedKey;
        try
        {
            salt = Convert.FromBase64String(segments[1]);
            expectedKey = Convert.FromBase64String(segments[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedKey.Length);
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }
}
