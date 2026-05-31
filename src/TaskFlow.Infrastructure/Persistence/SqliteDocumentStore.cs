using System.Text.Json;
using Microsoft.Data.Sqlite;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>
/// A <see cref="IDocumentStore"/> backed by a SQLite database, with one table per
/// collection holding each aggregate as an <c>(Id, Document)</c> row. It persists
/// the very same JSON shape as the file store, demonstrating that a different
/// storage technology plugs in behind the same abstraction with no change to the
/// repositories. Writes run inside a transaction so a collection is never left
/// half-updated.
/// </summary>
public sealed class SqliteDocumentStore : IDocumentStore
{
    private readonly string _connectionString;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public SqliteDocumentStore(string databasePath)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(databasePath));
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        // Bind the bundled native SQLite provider before any connection is opened.
        SQLitePCL.Batteries_V2.Init();

        _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();
    }

    public async Task<List<T>> ReadAllAsync<T>(string collection, CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await EnsureTableAsync(connection, collection, cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT Document FROM [{collection}]";

            var items = new List<T>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var item = JsonSerializer.Deserialize<T>(reader.GetString(0), DomainJson.Options);
                if (item is not null) items.Add(item);
            }
            return items;
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
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await EnsureTableAsync(connection, collection, cancellationToken);

            await using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

            await using (var delete = connection.CreateCommand())
            {
                delete.Transaction = transaction;
                delete.CommandText = $"DELETE FROM [{collection}]";
                await delete.ExecuteNonQueryAsync(cancellationToken);
            }

            foreach (var item in items)
            {
                await using var insert = connection.CreateCommand();
                insert.Transaction = transaction;
                insert.CommandText = $"INSERT INTO [{collection}] (Id, Document) VALUES ($id, $document)";
                insert.Parameters.AddWithValue("$id", item.Id.ToString());
                insert.Parameters.AddWithValue("$document", JsonSerializer.Serialize(item, DomainJson.Options));
                await insert.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static async Task EnsureTableAsync(SqliteConnection connection, string collection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS [{collection}] (Id TEXT PRIMARY KEY, Document TEXT NOT NULL)";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
