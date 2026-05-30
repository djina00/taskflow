using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Projects.Application.Queries.GetAllProjects;

public sealed class GetAllProjectsQueryHandler
    : IQueryHandler<GetAllProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IProjectRepository _projects;

    public GetAllProjectsQueryHandler(IProjectRepository projects) => _projects = projects;

    public async Task<IReadOnlyList<ProjectDto>> HandleAsync(
        GetAllProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var projects = await _projects.GetAllAsync(cancellationToken);
        return projects.Select(project => project.ToDto()).ToArray();
    }
}
