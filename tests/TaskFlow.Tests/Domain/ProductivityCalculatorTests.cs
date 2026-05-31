using TaskFlow.Modules.Reports.Domain;

namespace TaskFlow.Tests.Domain;

public class ProductivityCalculatorTests
{
    private static readonly Guid Project = Guid.NewGuid();
    private static readonly Guid User = Guid.NewGuid();

    [Fact]
    public void ForProject_counts_totals_and_computes_the_completion_rate()
    {
        var tasks = new[]
        {
            new TaskSnapshot(Guid.NewGuid(), Project, User, IsCompleted: true, PriorityWeight: 3),
            new TaskSnapshot(Guid.NewGuid(), Project, User, IsCompleted: false, PriorityWeight: 1),
            new TaskSnapshot(Guid.NewGuid(), Project, null, IsCompleted: false, PriorityWeight: 2),
        };

        var stats = ProductivityCalculator.ForProject(Project, tasks);

        Assert.Equal(3, stats.TotalTasks);
        Assert.Equal(1, stats.CompletedTasks);
        Assert.Equal(2, stats.OpenTasks);
        Assert.Equal(0.3333, stats.CompletionRate);   // rounded to 4 dp
    }

    [Fact]
    public void ForProject_with_no_tasks_reports_a_zero_rate()
    {
        var stats = ProductivityCalculator.ForProject(Project, Array.Empty<TaskSnapshot>());

        Assert.Equal(0, stats.TotalTasks);
        Assert.Equal(0d, stats.CompletionRate);
    }

    [Fact]
    public void ForUser_sums_points_of_completed_tasks_and_ignores_other_assignees()
    {
        var other = Guid.NewGuid();
        var tasks = new[]
        {
            new TaskSnapshot(Guid.NewGuid(), Project, User, IsCompleted: true, PriorityWeight: 3),
            new TaskSnapshot(Guid.NewGuid(), Project, User, IsCompleted: true, PriorityWeight: 2),
            new TaskSnapshot(Guid.NewGuid(), Project, User, IsCompleted: false, PriorityWeight: 1),
            new TaskSnapshot(Guid.NewGuid(), Project, other, IsCompleted: true, PriorityWeight: 3),
        };

        var productivity = ProductivityCalculator.ForUser(User, tasks);

        Assert.Equal(3, productivity.AssignedTasks);
        Assert.Equal(2, productivity.CompletedTasks);
        Assert.Equal(5, productivity.Points);            // 3 + 2, the other user's task excluded
        Assert.Equal(0.6667, productivity.CompletionRate);
    }
}
