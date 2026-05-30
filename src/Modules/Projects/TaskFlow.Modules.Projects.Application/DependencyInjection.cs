using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Modules.Projects.Application.Commands.AddProjectMember;
using TaskFlow.Modules.Projects.Application.Commands.ArchiveProject;
using TaskFlow.Modules.Projects.Application.Commands.CreateProject;
using TaskFlow.Modules.Projects.Application.Commands.RemoveProjectMember;
using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Application.Queries.GetAllProjects;
using TaskFlow.Modules.Projects.Application.Queries.GetProjectById;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Projects.Application;

/// <summary>
/// Composition entry point for the Projects module's application layer. Registers
/// the command and query handlers against their closed-generic interfaces so the
/// dispatchers can resolve them. Infrastructure adapters (the repository) are
/// registered separately by the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddProjectsModule(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateProjectCommand, Result<ProjectDto>>, CreateProjectCommandHandler>();
        services.AddScoped<ICommandHandler<AddProjectMemberCommand, Result>, AddProjectMemberCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveProjectMemberCommand, Result>, RemoveProjectMemberCommandHandler>();
        services.AddScoped<ICommandHandler<ArchiveProjectCommand, Result>, ArchiveProjectCommandHandler>();

        services.AddScoped<IQueryHandler<GetProjectByIdQuery, Result<ProjectDto>>, GetProjectByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllProjectsQuery, IReadOnlyList<ProjectDto>>, GetAllProjectsQueryHandler>();

        return services;
    }
}
