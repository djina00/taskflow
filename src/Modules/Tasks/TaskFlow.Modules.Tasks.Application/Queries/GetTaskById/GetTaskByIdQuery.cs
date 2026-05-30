using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Queries.GetTaskById;

/// <summary>Fetches a single task by identifier.</summary>
public sealed record GetTaskByIdQuery(Guid TaskId) : IQuery<Result<TaskItemDto>>;
