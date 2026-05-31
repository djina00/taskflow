using TaskFlow.Modules.Reports.Application.Abstractions;
using TaskFlow.Modules.Reports.Application.Contracts;
using TaskFlow.Modules.Reports.Domain;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Modules.Reports.Application.Queries.GetUserProductivity;

/// <summary>
/// Pulls every task snapshot through the port and lets the pure
/// <see cref="ProductivityCalculator"/> keep only those assigned to the user and
/// score them. An unknown user simply yields a zeroed score.
/// </summary>
public sealed class GetUserProductivityQueryHandler
    : IQueryHandler<GetUserProductivityQuery, UserProductivityDto>
{
    private readonly ITaskStatisticsProvider _statistics;

    public GetUserProductivityQueryHandler(ITaskStatisticsProvider statistics) => _statistics = statistics;

    public async Task<UserProductivityDto> HandleAsync(
        GetUserProductivityQuery query, CancellationToken cancellationToken = default)
    {
        var tasks = await _statistics.GetAllAsync(cancellationToken);
        return ProductivityCalculator.ForUser(query.UserId, tasks).ToDto();
    }
}
