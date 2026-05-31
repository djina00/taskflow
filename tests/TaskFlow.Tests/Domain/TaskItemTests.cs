using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.SharedKernel.Domain.Events;

namespace TaskFlow.Tests.Domain;

public class TaskItemTests
{
    private static TaskItem NewTask() =>
        TaskItem.Create(Guid.NewGuid(), "Punch the cards", null, TaskPriority.High).Value;

    [Fact]
    public void Create_requires_a_title()
    {
        var result = TaskItem.Create(Guid.NewGuid(), "   ", null, TaskPriority.Low);

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Validation", result.Error.Code);
    }

    [Fact]
    public void Start_is_only_allowed_from_the_todo_state()
    {
        var task = NewTask();

        Assert.True(task.Start().IsSuccess);
        Assert.Equal(TaskItemStatus.InProgress, task.Status);
        Assert.True(task.Start().IsFailure);   // already started
    }

    [Fact]
    public void Complete_marks_done_stamps_the_time_and_raises_the_event()
    {
        var task = NewTask();
        task.Assign(Guid.NewGuid());

        var result = task.Complete();

        Assert.True(result.IsSuccess);
        Assert.True(task.IsCompleted);
        Assert.NotNull(task.CompletedOnUtc);
        Assert.Contains(task.DomainEvents, e => e is TaskCompletedEvent);
    }

    [Fact]
    public void A_completed_task_cannot_be_reassigned()
    {
        var task = NewTask();
        task.Assign(Guid.NewGuid());
        task.Complete();

        var result = task.Assign(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Error.Conflict", result.Error.Code);
    }

    [Fact]
    public void Assign_raises_the_cross_module_event()
    {
        var task = NewTask();
        var assignee = Guid.NewGuid();

        Assert.True(task.Assign(assignee).IsSuccess);
        Assert.Equal(assignee, task.AssigneeId);
        Assert.Contains(task.DomainEvents, e => e is TaskAssignedEvent);
    }
}
