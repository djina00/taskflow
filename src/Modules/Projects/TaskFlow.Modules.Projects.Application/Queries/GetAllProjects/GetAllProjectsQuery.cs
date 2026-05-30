using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Projects.Application.Queries.GetAllProjects;

/// <summary>Lists all projects as read models.</summary>
public sealed record GetAllProjectsQuery : IQuery<IReadOnlyList<ProjectDto>>;
