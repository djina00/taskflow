using Microsoft.Extensions.DependencyInjection;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Infrastructure.Messaging;

/// <summary>
/// Routes a command to its single registered <see cref="ICommandHandler{TCommand,TResponse}"/>,
/// resolved from the DI container by the command's runtime type. This is the
/// custom, MediatR-free CQRS dispatcher: senders depend only on the
/// <see cref="ICommandDispatcher"/> port and never reference concrete handlers.
/// </summary>
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _provider;

    public CommandDispatcher(IServiceProvider provider) => _provider = provider;

    public Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = _provider.GetRequiredService(handlerType);
        var handle = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResponse>, TResponse>.HandleAsync))!;

        return (Task<TResponse>)handle.Invoke(handler, new object[] { command, cancellationToken })!;
    }
}
