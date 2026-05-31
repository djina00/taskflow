using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>
/// A minimal document-oriented persistence seam: each aggregate type is stored as
/// a collection of self-contained JSON documents. The repositories depend only on
/// this abstraction, so the concrete storage technology — a JSON file or a SQLite
/// database — is an interchangeable adapter chosen at the composition root. This is
/// the Dependency-Inversion seam that lets the whole application swap persistence
/// without touching a single line of domain or use-case code.
/// </summary>
public interface IDocumentStore
{
    Task<List<T>> ReadAllAsync<T>(string collection, CancellationToken cancellationToken = default)
        where T : BaseEntity;

    Task WriteAllAsync<T>(string collection, IReadOnlyList<T> items, CancellationToken cancellationToken = default)
        where T : BaseEntity;
}
