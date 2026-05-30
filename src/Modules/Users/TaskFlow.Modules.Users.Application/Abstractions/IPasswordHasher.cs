namespace TaskFlow.Modules.Users.Application.Abstractions;

/// <summary>
/// Port for turning a plaintext password into a storable hash and verifying a
/// candidate password against one. Declared in the Application layer because
/// that is where it is consumed; the cryptographic implementation is an
/// Infrastructure concern supplied via Dependency Injection. Keeping hashing
/// behind this seam means the domain and use cases never touch plaintext crypto.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
