namespace TaskFlow.Modules.Tasks.Domain;

/// <summary>
/// The workflow state of a <see cref="TaskItem"/>. Named <c>TaskItemStatus</c>
/// rather than <c>TaskStatus</c> to avoid clashing with
/// <see cref="System.Threading.Tasks.TaskStatus"/>.
/// </summary>
public enum TaskItemStatus
{
    Todo = 1,
    InProgress = 2,
    Done = 3
}
