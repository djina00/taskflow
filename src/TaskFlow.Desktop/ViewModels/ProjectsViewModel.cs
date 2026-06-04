using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Modules.Projects.Application.Commands.CreateProject;
using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Application.Queries.GetAllProjects;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the Projects tab: create a project owned by the signed-in user, list all
/// projects, and choose the one the Tasks and Reports tabs work against.
/// </summary>
public sealed class ProjectsViewModel : ViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;
    private readonly SessionContext _session;

    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _status = string.Empty;
    private ProjectDto? _selectedProject;

    public ProjectsViewModel(ICommandDispatcher commands, IQueryDispatcher queries, SessionContext session)
    {
        _commands = commands;
        _queries = queries;
        _session = session;

        CreateCommand = new AsyncRelayCommand(CreateAsync, () => _session.IsLoggedIn);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        _session.PropertyChanged += (_, _) => ((AsyncRelayCommand)CreateCommand).RaiseCanExecuteChanged();
    }

    public SessionContext Session => _session;

    public ObservableCollection<ProjectDto> Projects { get; } = new();

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public ProjectDto? SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (SetProperty(ref _selectedProject, value) && value is not null)
                _session.SelectProject(value.Id, value.Name);
        }
    }

    public ICommand CreateCommand { get; }
    public ICommand RefreshCommand { get; }

    private async Task CreateAsync()
    {
        if (_session.CurrentUserId is not Guid ownerId)
        {
            Status = "Sign in first.";
            return;
        }

        var result = await _commands.SendAsync(new CreateProjectCommand(Name, ownerId, Description));
        if (result.IsFailure)
        {
            Status = result.Error.Message;
            return;
        }

        Status = $"Created project '{result.Value.Name}'.";
        Name = string.Empty;
        Description = string.Empty;
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        var projects = await _queries.QueryAsync(new GetAllProjectsQuery());
        Projects.Clear();
        foreach (var project in projects)
            Projects.Add(project);
    }
}
