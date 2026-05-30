using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application.Queries.GetProjectById;

/// <summary>Fetches a single project by identifier.</summary>
public sealed record GetProjectByIdQuery(Guid ProjectId) : IQuery<Result<ProjectDto>>;
