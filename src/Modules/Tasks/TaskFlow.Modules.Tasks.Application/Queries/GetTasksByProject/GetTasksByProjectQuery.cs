using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Tasks.Application.Queries.GetTasksByProject;

/// <summary>Lists the tasks belonging to a project as read models.</summary>
public sealed record GetTasksByProjectQuery(Guid ProjectId) : IQuery<IReadOnlyList<TaskItemDto>>;
