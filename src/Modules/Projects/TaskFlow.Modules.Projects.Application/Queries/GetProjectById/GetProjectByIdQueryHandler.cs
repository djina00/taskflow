using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Domain;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler
    : IQueryHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    private readonly IProjectRepository _projects;

    public GetProjectByIdQueryHandler(IProjectRepository projects) => _projects = projects;

    public async Task<Result<ProjectDto>> HandleAsync(
        GetProjectByIdQuery query, CancellationToken cancellationToken = default)
    {
        var project = await _projects.GetByIdAsync(query.ProjectId, cancellationToken);
        return project is null
            ? Error.NotFound($"Project '{query.ProjectId}' was not found.")
            : project.ToDto();
    }
}
