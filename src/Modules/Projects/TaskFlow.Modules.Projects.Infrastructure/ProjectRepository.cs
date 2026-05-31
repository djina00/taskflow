using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Projects.Infrastructure;

/// <summary>
/// Persistence adapter for the <see cref="Project"/> aggregate. Reuses the generic
/// <see cref="DocumentRepository{T}"/> for CRUD and adds the membership lookup the
/// use cases need.
/// </summary>
public sealed class ProjectRepository : DocumentRepository<Project>, IProjectRepository
{
    public ProjectRepository(IDocumentStore store, IDomainEventDispatcher dispatcher)
        : base(store, dispatcher) { }

    public async Task<IReadOnlyList<Project>> GetByMemberAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var all = await LoadAsync(cancellationToken);
        return all.Where(project => project.HasMember(userId)).ToList();
    }
}
