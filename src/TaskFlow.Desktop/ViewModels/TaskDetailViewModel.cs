using System.Collections.ObjectModel;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Desktop.Services;
using TaskFlow.Modules.Tasks.Application.Commands.AssignTask;
using TaskFlow.Modules.Tasks.Application.Commands.CompleteTask;
using TaskFlow.Modules.Tasks.Application.Commands.StartTask;
using TaskFlow.Modules.Tasks.Application.Commands.UpdateTask;
using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.Modules.Tasks.Application.Queries.GetTaskById;
using TaskFlow.Modules.Users.Application.Contracts;
using TaskFlow.Modules.Users.Application.Queries.GetAllUsers;
using TaskFlow.SharedKernel.Messaging;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Backs the task detail dialog opened from the Tasks list: everything you do to an
/// existing task lives here — edit its title/description/priority, assign it to a
/// user and move it through its lifecycle (start, complete). Editing closes the dialog;
/// lifecycle and assignment actions stay open and update the shown status, and the
/// dialog reports back that the task changed so the list refreshes. (Deletion lives on
/// the task's row in the list, not here.)
/// </summary>
public sealed class TaskDetailViewModel : ViewModelBase, IDialogViewModel
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;

    private TaskItemDto _task;
    private string _title;
    private string _description;
    private string _priority;
    private string _statusDisplay;
    private string _reporterName = string.Empty;
    private UserDto? _selectedAssignee;
    private string _status = string.Empty;
    private bool _changed;

    public TaskDetailViewModel(TaskItemDto task, ICommandDispatcher commands, IQueryDispatcher queries)
    {
        _task = task;
        _commands = commands;
        _queries = queries;
        _title = task.Title;
        _description = task.Description;
        _priority = task.Priority;
        _statusDisplay = task.Status;

        SaveCommand = new AsyncRelayCommand(SaveAsync, () => !string.IsNullOrWhiteSpace(Title));
        AssignCommand = new AsyncRelayCommand(AssignAsync, () => _selectedAssignee is not null);
        StartCommand = new AsyncRelayCommand(() => RunLifecycleAsync(new StartTaskCommand(_task.Id), "Started."));
        CompleteCommand = new AsyncRelayCommand(() => RunLifecycleAsync(new CompleteTaskCommand(_task.Id), "Completed."));
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(_changed));
    }

    public event Action<bool>? CloseRequested;
    public string DialogTitle => $"Edit task — {_task.Title}";

    /// <summary>All users, for the assignee picker.</summary>
    public ObservableCollection<UserDto> Users { get; } = new();

    public string Title
    {
        get => _title;
        set { if (SetProperty(ref _title, value)) SaveCommand.RaiseCanExecuteChanged(); }
    }

    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string Priority { get => _priority; set => SetProperty(ref _priority, value); }

    /// <summary>The task's current workflow status, shown read-only and refreshed after each action.</summary>
    public string StatusDisplay { get => _statusDisplay; private set => SetProperty(ref _statusDisplay, value); }

    /// <summary>The reporter's name, shown read-only.</summary>
    public string ReporterName { get => _reporterName; private set => SetProperty(ref _reporterName, value); }

    /// <summary>The latest outcome message for an action in the dialog.</summary>
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public UserDto? SelectedAssignee
    {
        get => _selectedAssignee;
        set { if (SetProperty(ref _selectedAssignee, value)) AssignCommand.RaiseCanExecuteChanged(); }
    }

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand AssignCommand { get; }
    public IRelayCommand StartCommand { get; }
    public IRelayCommand CompleteCommand { get; }
    public IRelayCommand CloseCommand { get; }

    /// <summary>Loads the user list and resolves the reporter and current assignee.</summary>
    public async Task LoadAsync()
    {
        var users = await _queries.QueryAsync(new GetAllUsersQuery());
        Users.ReplaceAll(users);
        var names = users.ToDictionary(user => user.Id, user => user.FullName);
        ReporterName = names.TryGetValue(_task.ReporterId, out var name) ? name : "Unknown";
        SelectedAssignee = _task.AssigneeId is Guid id ? Users.FirstOrDefault(user => user.Id == id) : null;
    }

    private async Task SaveAsync()
    {
        var result = await _commands.SendAsync(new UpdateTaskCommand(_task.Id, Title, Description, Priority));
        if (result.IsFailure)
        {
            Status = result.Error.Message;
            return;
        }

        CloseRequested?.Invoke(true);
    }

    private async Task AssignAsync()
    {
        if (_selectedAssignee is not { } user)
            return;

        var result = await _commands.SendAsync(new AssignTaskCommand(_task.Id, user.Id));
        Status = result.IsFailure ? result.Error.Message : $"Assigned to {user.FullName}.";
        if (result.IsSuccess)
        {
            _changed = true;
            await ReloadAsync();
        }
    }

    private async Task RunLifecycleAsync(ICommand<Result> command, string success)
    {
        var result = await _commands.SendAsync(command);
        Status = result.IsFailure ? result.Error.Message : success;
        if (result.IsSuccess)
        {
            _changed = true;
            await ReloadAsync();
        }
    }

    private async Task ReloadAsync()
    {
        var result = await _queries.QueryAsync(new GetTaskByIdQuery(_task.Id));
        if (result.IsFailure)
            return;

        _task = result.Value;
        StatusDisplay = _task.Status;
        SelectedAssignee = _task.AssigneeId is Guid id ? Users.FirstOrDefault(user => user.Id == id) : null;
    }
}
