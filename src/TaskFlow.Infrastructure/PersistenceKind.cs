namespace TaskFlow.Infrastructure;

/// <summary>
/// Selects which interchangeable persistence adapter the composition root wires
/// up. Both implement the same document-store contract, so switching between them
/// is a one-line change with no impact on the rest of the system.
/// </summary>
public enum PersistenceKind
{
    /// <summary>One human-readable JSON file per aggregate collection.</summary>
    Json,

    /// <summary>A SQLite database with one document table per aggregate collection.</summary>
    Sqlite
}
