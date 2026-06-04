using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Modules.Tasks.Application.Commands.AssignTask;
using TaskFlow.Modules.Tasks.Application.Commands.CompleteTask;
using TaskFlow.Modules.Tasks.Application.Commands.CreateTask;
using TaskFlow.Modules.Tasks.Application.Commands.StartTask;
using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.Modules.Tasks.Application.Queries.GetTasksByProject;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the Tasks tab for the currently selected project: create tasks and move
/// the selected task through its lifecycle (start, complete, assign-to-me).
/// Assigning or completing a task raises the cross-module events that the
/// Notifications tab then surfaces.
/// </summary>
public sealed class TasksViewModel : ViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;
    private readonly SessionContext _session;

    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _priority = "Medium";
    private string _status = string.Empty;
    private TaskItemDto? _selectedTask;

    public TasksViewModel(ICommandDispatcher commands, IQueryDispatcher queries, SessionContext session)
    {
        _commands = commands;
        _queries = queries;
        _session = session;

        CreateCommand = new AsyncRelayCommand(CreateAsync, () => _session.HasSelectedProject);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => _session.HasSelectedProject);
        AssignToMeCommand = new AsyncRelayCommand(() => RunOnSelectedAsync(id => new AssignTaskCommand(id, _session.CurrentUserId ?? Guid.Empty)), CanActOnTask);
        StartCommand = new AsyncRelayCommand(() => RunOnSelectedAsync(id => new StartTaskCommand(id)), CanActOnTask);
        CompleteCommand = new AsyncRelayCommand(() => RunOnSelectedAsync(id => new CompleteTaskCommand(id)), CanActOnTask);

        _session.PropertyChanged += async (_, _) =>
        {
            RaiseAll();
            if (_session.HasSelectedProject)
                await RefreshAsync();
        };
    }

    public SessionContext Session => _session;

    public ObservableCollection<TaskItemDto> Tasks { get; } = new();

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string Priority { get => _priority; set => SetProperty(ref _priority, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public TaskItemDto? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (SetProperty(ref _selectedTask, value))
                RaiseAll();
        }
    }

    public ICommand CreateCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AssignToMeCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand CompleteCommand { get; }

    private bool CanActOnTask() => _selectedTask is not null && _session.IsLoggedIn;

    private async Task CreateAsync()
    {
        if (_session.SelectedProjectId is not Guid projectId)
        {
            Status = "Select a project on the Projects tab first.";
            return;
        }

        var result = await _commands.SendAsync(new CreateTaskCommand(projectId, Title, Description, Priority));
        if (result.IsFailure)
        {
            Status = result.Error.Message;
            return;
        }

        Status = $"Created task '{result.Value.Title}'.";
        Title = string.Empty;
        Description = string.Empty;
        await RefreshAsync();
    }

    private async Task RunOnSelectedAsync(Func<Guid, ICommand<Result>> commandFactory)
    {
        if (_selectedTask is null)
            return;

        var result = await _commands.SendAsync(commandFactory(_selectedTask.Id));
        Status = result.IsFailure ? result.Error.Message : "Done.";
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        if (_session.SelectedProjectId is not Guid projectId)
        {
            Tasks.Clear();
            return;
        }

        var tasks = await _queries.QueryAsync(new GetTasksByProjectQuery(projectId));
        Tasks.Clear();
        foreach (var task in tasks)
            Tasks.Add(task);
    }

    private void RaiseAll()
    {
        ((AsyncRelayCommand)CreateCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)RefreshCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)AssignToMeCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)StartCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)CompleteCommand).RaiseCanExecuteChanged();
    }
}
