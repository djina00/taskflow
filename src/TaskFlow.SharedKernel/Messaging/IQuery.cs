namespace TaskFlow.SharedKernel.Messaging;

/// <summary>
/// Marker for a CQRS query — a request that reads state without changing it
/// (read side). <typeparamref name="TResponse"/> is the data returned.
/// </summary>
public interface IQuery<TResponse>
{
}

/// <summary>
/// Handles a single query type. Separating queries from commands lets the read
/// and write sides evolve independently (CQRS).
/// </summary>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Port that routes a query to its registered handler. Implemented by an
/// infrastructure adapter that resolves the handler from the DI container.
/// </summary>
public interface IQueryDispatcher
{
    Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
