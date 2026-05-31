using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Messaging;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Infrastructure;

/// <summary>
/// Composition entry point for the cross-cutting infrastructure: the CQRS and
/// domain-event dispatchers, and the chosen <see cref="IDocumentStore"/> adapter.
/// Each module registers its own repositories and adapters through its
/// <c>AddXInfrastructure</c> extension; this method wires the shared plumbing they
/// all build on.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, PersistenceKind persistence, string dataDirectory)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddSingleton<IDocumentStore>(persistence switch
        {
            PersistenceKind.Sqlite => new SqliteDocumentStore(Path.Combine(dataDirectory, "taskflow.db")),
            _ => new JsonFileDocumentStore(dataDirectory)
        });

        return services;
    }
}
