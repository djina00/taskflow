using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application.Commands.AddComment;

/// <summary>Adds a comment authored by a user to a task.</summary>
public sealed record AddCommentCommand(Guid TaskId, Guid AuthorId, string Text) : ICommand<Result>;
