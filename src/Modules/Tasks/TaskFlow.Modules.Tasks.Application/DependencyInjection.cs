using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Tasks.Application.Commands.AddComment;
using TaskFlow.Modules.Tasks.Application.Commands.AssignTask;
using TaskFlow.Modules.Tasks.Application.Commands.CompleteTask;
using TaskFlow.Modules.Tasks.Application.Commands.CreateTask;
using TaskFlow.Modules.Tasks.Application.Commands.DeleteTask;
using TaskFlow.Modules.Tasks.Application.Commands.StartTask;
using TaskFlow.Modules.Tasks.Application.Commands.UpdateTask;
using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.Modules.Tasks.Application.Queries.GetTaskById;
using TaskFlow.Modules.Tasks.Application.Queries.GetTasksByProject;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Tasks.Application;

/// <summary>
/// Composition entry point for the Tasks module's application layer. Registers
/// the command and query handlers against their closed-generic interfaces so the
/// dispatchers can resolve them. Infrastructure adapters (the repository) are
/// registered separately by the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddTasksModule(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateTaskCommand, Result<TaskItemDto>>, CreateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<AssignTaskCommand, Result>, AssignTaskCommandHandler>();
        services.AddScoped<ICommandHandler<StartTaskCommand, Result>, StartTaskCommandHandler>();
        services.AddScoped<ICommandHandler<CompleteTaskCommand, Result>, CompleteTaskCommandHandler>();
        services.AddScoped<ICommandHandler<AddCommentCommand, Result>, AddCommentCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskCommand, Result>, UpdateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTaskCommand, Result>, DeleteTaskCommandHandler>();

        services.AddScoped<IQueryHandler<GetTaskByIdQuery, Result<TaskItemDto>>, GetTaskByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetTasksByProjectQuery, IReadOnlyList<TaskItemDto>>, GetTasksByProjectQueryHandler>();

        return services;
    }
}
