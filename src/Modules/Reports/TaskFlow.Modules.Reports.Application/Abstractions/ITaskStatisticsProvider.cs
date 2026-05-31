using TaskFlow.Modules.Reports.Domain;

namespace TaskFlow.Modules.Reports.Application.Abstractions;

/// <summary>
/// The Reports module's read-side port for obtaining task facts. By depending on
/// this abstraction — rather than on the Tasks module — Reports stays decoupled:
/// the Infrastructure layer supplies an adapter that reads from the task store
/// and maps each task into a Reports-owned <see cref="TaskSnapshot"/>.
/// </summary>
public interface ITaskStatisticsProvider
{
    Task<IReadOnlyList<TaskSnapshot>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskSnapshot>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
}
