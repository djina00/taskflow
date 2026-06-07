using System.Collections.ObjectModel;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Desktop.Services;
using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Application.Queries.GetAllProjects;
using TaskFlow.Modules.Tasks.Application.Commands.CreateTask;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Backs the "new task" dialog opened from the Tasks list. It picks the target project
/// (defaulting to the one currently filtered in the list), collects the title,
/// description and priority, creates the task (reported by the signed-in user) and
/// raises <see cref="CloseRequested"/> with <c>true</c> on success so the list refreshes.
/// </summary>
public sealed class TaskCreateViewModel : ViewModelBase, IDialogViewModel
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;
    private readonly SessionContext _session;

    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _priority = "Medium";
    private ProjectDto? _selectedProject;
    private string _status = string.Empty;

    public TaskCreateViewModel(ICommandDispatcher commands, IQueryDispatcher queries, SessionContext session)
    {
        _commands = commands;
        _queries = queries;
        _session = session;

        CreateCommand = new AsyncRelayCommand(CreateAsync,
            () => !string.IsNullOrWhiteSpace(Title) && _selectedProject is not null);
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }

    public event Action<bool>? CloseRequested;
    public string DialogTitle => "New task";

    /// <summary>The projects the task can be created in.</summary>
    public ObservableCollection<ProjectDto> Projects { get; } = new();

    public ProjectDto? SelectedProject
    {
        get => _selectedProject;
        set { if (SetProperty(ref _selectedProject, value)) CreateCommand.RaiseCanExecuteChanged(); }
    }

    public string Title
    {
        get => _title;
        set { if (SetProperty(ref _title, value)) CreateCommand.RaiseCanExecuteChanged(); }
    }

    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string Priority { get => _priority; set => SetProperty(ref _priority, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public IRelayCommand CreateCommand { get; }
    public IRelayCommand CloseCommand { get; }

    /// <summary>
    /// Loads the project list and pre-selects <paramref name="preferredProjectId"/> (the
    /// project being viewed in the list), falling back to the first project.
    /// </summary>
    public async Task LoadAsync(Guid? preferredProjectId)
    {
        Projects.ReplaceAll(await _queries.QueryAsync(new GetAllProjectsQuery()));
        SelectedProject = Projects.FirstOrDefault(project => project.Id == preferredProjectId)
            ?? Projects.FirstOrDefault();
    }

    private async Task CreateAsync()
    {
        if (_selectedProject is not { } project)
        {
            Status = "Pick a project first.";
            return;
        }
        if (_session.CurrentUserId is not Guid reporterId)
        {
            Status = "Sign in first.";
            return;
        }

        var result = await _commands.SendAsync(
            new CreateTaskCommand(project.Id, Title, Description, Priority, reporterId));
        if (result.IsFailure)
        {
            Status = result.Error.Message;
            return;
        }

        CloseRequested?.Invoke(true);
    }
}
