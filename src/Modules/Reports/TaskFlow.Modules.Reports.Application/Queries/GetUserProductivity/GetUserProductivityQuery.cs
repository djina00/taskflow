using TaskFlow.Modules.Reports.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Reports.Application.Queries.GetUserProductivity;

/// <summary>Produces a user's productivity score across all projects.</summary>
public sealed record GetUserProductivityQuery(Guid UserId) : IQuery<UserProductivityDto>;
