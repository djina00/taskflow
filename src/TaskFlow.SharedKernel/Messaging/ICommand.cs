namespace TaskFlow.SharedKernel.Messaging;

/// <summary>
/// Marker for a CQRS command — a request that changes state (write side).
/// <typeparamref name="TResponse"/> is typically a <c>Result</c> or
/// <c>Result&lt;T&gt;</c>.
/// </summary>
public interface ICommand<TResponse>
{
}

/// <summary>
/// Handles a single command type. One handler per command keeps write use
/// cases small, focused and individually testable (Single Responsibility).
/// </summary>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Port that routes a command to its registered handler. Implemented by an
/// infrastructure adapter that resolves the handler from the DI container.
/// </summary>
public interface ICommandDispatcher
{
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
