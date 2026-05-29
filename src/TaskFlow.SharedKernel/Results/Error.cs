namespace TaskFlow.SharedKernel.Results;

/// <summary>
/// A structured error with a machine-readable code and a human-readable
/// message. Used by <see cref="Result"/> to describe failures without throwing
/// exceptions for expected, recoverable conditions.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error Validation(string message) => new("Error.Validation", message);

    public static Error NotFound(string message) => new("Error.NotFound", message);

    public static Error Conflict(string message) => new("Error.Conflict", message);

    public static Error Unauthorized(string message) => new("Error.Unauthorized", message);

    public override string ToString() => string.IsNullOrEmpty(Code) ? Message : $"{Code}: {Message}";
}
