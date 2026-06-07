using TaskFlow.Modules.Tasks.Application.Contracts;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// A task as shown in the Tasks list. The Tasks module only knows the reporter and
/// assignee by id; this presentation row pairs the task with the names resolved from
/// the Users module, so the grid shows people rather than identifiers.
/// </summary>
public sealed record TaskListItem(TaskItemDto Task, string ReporterName, string AssigneeName);
