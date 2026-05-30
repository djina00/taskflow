using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Modules.Projects.Domain;

/// <summary>
/// Persistence port for the <see cref="Project"/> aggregate. Extends the generic
/// <see cref="IRepository{T}"/> with the lookups the Projects use cases need —
/// finding the projects a given user participates in. Concrete adapters live in
/// the Infrastructure layer and are supplied via Dependency Injection.
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    Task<IReadOnlyList<Project>> GetByMemberAsync(Guid userId, CancellationToken cancellationToken = default);
}
