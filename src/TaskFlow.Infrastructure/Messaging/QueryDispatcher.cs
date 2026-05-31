using Microsoft.Extensions.DependencyInjection;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Infrastructure.Messaging;

/// <summary>
/// Routes a query to its single registered <see cref="IQueryHandler{TQuery,TResponse}"/>,
/// resolved from the DI container by the query's runtime type. The read-side twin
/// of <see cref="CommandDispatcher"/>, keeping callers decoupled from concrete
/// query handlers.
/// </summary>
public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _provider;

    public QueryDispatcher(IServiceProvider provider) => _provider = provider;

    public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = _provider.GetRequiredService(handlerType);
        var handle = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResponse>, TResponse>.HandleAsync))!;

        return (Task<TResponse>)handle.Invoke(handler, new object[] { query, cancellationToken })!;
    }
}
