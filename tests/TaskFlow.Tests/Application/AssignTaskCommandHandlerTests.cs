using TaskFlow.Modules.Tasks.Application.Commands.AssignTask;
using TaskFlow.Modules.Tasks.Domain;
using TaskFlow.Tests.Fakes;

namespace TaskFlow.Tests.Application;

public class AssignTaskCommandHandlerTests
{
    private readonly InMemoryTaskRepository _tasks = new();

    private AssignTaskCommandHandler CreateHandler() => new(_tasks);

    [Fact]
    public async Task Assigns_an_existing_task_to_a_user()
    {
        var task = TaskItem.Create(Guid.NewGuid(), "Punch the cards", null, TaskPriority.Medium, Guid.NewGuid()).Value;
        await _tasks.AddAsync(task);
        var assignee = Guid.NewGuid();

        var result = await CreateHandler().HandleAsync(new AssignTaskCommand(task.Id, assignee));

        Assert.True(result.IsSuccess);
        var reloaded = await _tasks.GetByIdAsync(task.Id);
        Assert.Equal(assignee, reloaded!.AssigneeId);
    }

    [Fact]
    public async Task Fails_when_the_task_does_not_exist()
    {
        var result = await CreateHandler().HandleAsync(new AssignTaskCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsFailure);
        Assert.Equal("Error.NotFound", result.Error.Code);
    }
}
