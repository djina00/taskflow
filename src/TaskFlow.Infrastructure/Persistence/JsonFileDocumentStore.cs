using System.Text.Json;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>
/// A <see cref="IDocumentStore"/> backed by one JSON file per collection inside a
/// data directory. Human-readable and dependency-free, it is the default
/// persistence adapter for the desktop app. A per-store lock serialises reads and
/// writes so concurrent operations never corrupt a file.
/// </summary>
public sealed class JsonFileDocumentStore : IDocumentStore
{
    private readonly string _directory;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonFileDocumentStore(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(directory);
    }

    private string PathFor(string collection) => Path.Combine(_directory, $"{collection}.json");

    public async Task<List<T>> ReadAllAsync<T>(string collection, CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var path = PathFor(collection);
            if (!File.Exists(path)) return new List<T>();

            await using var stream = File.OpenRead(path);
            var items = await JsonSerializer.DeserializeAsync<List<T>>(stream, DomainJson.Options, cancellationToken);
            return items ?? new List<T>();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task WriteAllAsync<T>(string collection, IReadOnlyList<T> items, CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var stream = File.Create(PathFor(collection));
            await JsonSerializer.SerializeAsync(stream, items, DomainJson.Options, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }
}
