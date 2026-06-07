using System.Collections.ObjectModel;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Desktop.Services;
using TaskFlow.Modules.Tasks.Application.Commands.DeleteTask;
using TaskFlow.Modules.Tasks.Application.Contracts;
using TaskFlow.Modules.Tasks.Application.Queries.GetTasksByProject;
using TaskFlow.Modules.Users.Application.Queries.GetAllUsers;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the Tasks section: a project picker at the top chooses which project's tasks
/// to show (defaulting to the first project) and an "Add task" button opens the
/// creation dialog. The list shows the project's tasks with reporter and assignee
/// names. Double-clicking a task opens its detail dialog, where it is edited, assigned
/// or moved through its lifecycle, and each row carries a delete action.
/// </summary>
public sealed class TasksViewModel : FeatureViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;
    private readonly Func<TaskItemDto, TaskDetailViewModel> _detailFactory;
    private readonly Func<TaskCreateViewModel> _createFactory;
    private readonly IDialogService _dialogs;
    private readonly IConfirmationService _confirm;

    private TaskListItem? _selectedTask;
    private Dictionary<Guid, string> _userNames = new();

    public TasksViewModel(ICommandDispatcher commands, IQueryDispatcher queries, SessionContext session,
        ProjectSelectionViewModel projectSelection, Func<TaskItemDto, TaskDetailViewModel> detailFactory,
        Func<TaskCreateViewModel> createFactory, IDialogService dialogs, IConfirmationService confirm)
        : base(session)
    {
        _commands = commands;
        _queries = queries;
        _detailFactory = detailFactory;
        _createFactory = createFactory;
        _dialogs = dialogs;
        _confirm = confirm;
        ProjectSelection = projectSelection;

        AddCommand = new AsyncRelayCommand(AddAsync, () => Session.HasSelectedProject);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => Session.HasSelectedProject);
        OpenSelectedCommand = new AsyncRelayCommand(OpenSelectedAsync, () => _selectedTask is not null);
        DeleteCommand = new AsyncRelayCommand<TaskListItem>(DeleteAsync);

        // The selected project lives in the session; reload the list when it — or the
        // sign-in state — changes, and re-evaluate the add/refresh buttons.
        Session.PropertyChanged += async (_, _) =>
        {
            AddCommand.RaiseCanExecuteChanged();
            RefreshCommand.RaiseCanExecuteChanged();
            if (Session.HasSelectedProject)
                await RefreshAsync();
        };
    }

    /// <summary>The shared project chooser shown at the top of the page.</summary>
    public ProjectSelectionViewModel ProjectSelection { get; }

    public ObservableCollection<TaskListItem> Tasks { get; } = new();

    /// <summary>The task picked in the list; double-clicking it opens its detail dialog.</summary>
    public TaskListItem? SelectedTask
    {
        get => _selectedTask;
        set { if (SetProperty(ref _selectedTask, value)) OpenSelectedCommand.RaiseCanExecuteChanged(); }
    }

    public IRelayCommand AddCommand { get; }
    public IRelayCommand RefreshCommand { get; }
    public IRelayCommand OpenSelectedCommand { get; }
    public IRelayCommand DeleteCommand { get; }

    public override async Task OnActivatedAsync()
    {
        await LoadUsersAsync();
        await ProjectSelection.LoadAsync();

        // Default the picker to the first project so the list shows tasks straight away
        // instead of waiting for the user to choose a project.
        if (ProjectSelection.Selected is null && ProjectSelection.Projects.Count > 0)
            ProjectSelection.Selected = ProjectSelection.Projects[0];

        await RefreshAsync();
    }

    private async Task AddAsync()
    {
        var dialog = _createFactory();
        await dialog.LoadAsync(Session.SelectedProjectId);
        if (_dialogs.ShowDialog(dialog))
            await RefreshAsync();
    }

    private async Task OpenSelectedAsync()
    {
        if (_selectedTask is not { } row)
            return;

        var detail = _detailFactory(row.Task);
        await detail.LoadAsync();
        if (_dialogs.ShowDialog(detail))
            await RefreshAsync();
    }

    // Deletes a task straight from its row in the list, after a confirmation prompt, so
    // removing a task no longer requires opening its detail dialog.
    private async Task DeleteAsync(TaskListItem row)
    {
        if (!_confirm.Confirm("Delete task", $"Delete task '{row.Task.Title}'? This cannot be undone."))
            return;

        await RunAsync(
            () => _commands.SendAsync(new DeleteTaskCommand(row.Task.Id)),
            $"Deleted task '{row.Task.Title}'.");
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        if (Session.SelectedProjectId is not Guid projectId)
        {
            Tasks.Clear();
            return;
        }

        if (_userNames.Count == 0)
            await LoadUsersAsync();

        var tasks = await _queries.QueryAsync(new GetTasksByProjectQuery(projectId));
        Tasks.ReplaceAll(tasks.Select(task =>
            new TaskListItem(task, ReporterNameOf(task.ReporterId), AssigneeNameOf(task.AssigneeId))));
    }

    private async Task LoadUsersAsync()
    {
        var users = await _queries.QueryAsync(new GetAllUsersQuery());
        _userNames = users.ToDictionary(user => user.Id, user => user.FullName);
    }

    private string ReporterNameOf(Guid reporterId) =>
        _userNames.TryGetValue(reporterId, out var name) ? name : "Unknown";

    private string AssigneeNameOf(Guid? assigneeId) =>
        assigneeId is Guid id
            ? _userNames.TryGetValue(id, out var name) ? name : "Unknown"
            : "Unassigned";
}
