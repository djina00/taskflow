using System.Collections.ObjectModel;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Desktop.Services;
using TaskFlow.Modules.Projects.Application.Commands.DeleteProject;
using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Application.Queries.GetAllProjects;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the Projects section: the list shows every project full-width, with an
/// "Add project" button above it that opens the creation dialog. Double-clicking a
/// project opens its detail dialog for editing, and each row carries a delete action —
/// so managing a project happens on the project itself, never in an inline form.
/// </summary>
public sealed class ProjectsViewModel : FeatureViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;
    private readonly Func<ProjectDto, ProjectDetailViewModel> _detailFactory;
    private readonly Func<ProjectCreateViewModel> _createFactory;
    private readonly IDialogService _dialogs;
    private readonly IConfirmationService _confirm;

    private ProjectDto? _selectedProject;

    public ProjectsViewModel(ICommandDispatcher commands, IQueryDispatcher queries, SessionContext session,
        Func<ProjectDto, ProjectDetailViewModel> detailFactory, Func<ProjectCreateViewModel> createFactory,
        IDialogService dialogs, IConfirmationService confirm)
        : base(session)
    {
        _commands = commands;
        _queries = queries;
        _detailFactory = detailFactory;
        _createFactory = createFactory;
        _dialogs = dialogs;
        _confirm = confirm;

        AddCommand = new AsyncRelayCommand(AddAsync, () => Session.IsLoggedIn);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        OpenSelectedCommand = new AsyncRelayCommand(OpenSelectedAsync, () => _selectedProject is not null);
        DeleteCommand = new AsyncRelayCommand<ProjectDto>(DeleteAsync);
        WireSessionToCommands(AddCommand);
    }

    public ObservableCollection<ProjectDto> Projects { get; } = new();

    /// <summary>The project picked in the list; double-clicking it opens its detail dialog.</summary>
    public ProjectDto? SelectedProject
    {
        get => _selectedProject;
        set { if (SetProperty(ref _selectedProject, value)) OpenSelectedCommand.RaiseCanExecuteChanged(); }
    }

    public IRelayCommand AddCommand { get; }
    public IRelayCommand RefreshCommand { get; }
    public IRelayCommand OpenSelectedCommand { get; }
    public IRelayCommand DeleteCommand { get; }

    public override Task OnActivatedAsync() => RefreshAsync();

    private async Task AddAsync()
    {
        if (_dialogs.ShowDialog(_createFactory()))
            await RefreshAsync();
    }

    private async Task OpenSelectedAsync()
    {
        if (_selectedProject is not { } project)
            return;

        if (_dialogs.ShowDialog(_detailFactory(project)))
            await RefreshAsync();
    }

    // Deletes a project straight from its row in the list, after a confirmation prompt,
    // so removing a project no longer requires opening its detail dialog.
    private async Task DeleteAsync(ProjectDto project)
    {
        if (!_confirm.Confirm("Delete project", $"Delete project '{project.Name}'? This cannot be undone."))
            return;

        await RunAsync(
            () => _commands.SendAsync(new DeleteProjectCommand(project.Id)),
            $"Deleted project '{project.Name}'.");
        await RefreshAsync();
    }

    private async Task RefreshAsync() =>
        Projects.ReplaceAll(await _queries.QueryAsync(new GetAllProjectsQuery()));
}
