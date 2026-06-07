using System.Collections.ObjectModel;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.Modules.Projects.Application.Queries.GetAllProjects;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// The project chooser shared by the sections that work against a single project
/// (Tasks and Reports). It is a single instance so both pickers show the same list
/// and stay in step, and choosing a project here drives the shared
/// <see cref="SessionContext"/> selection that those features read.
/// </summary>
public sealed class ProjectSelectionViewModel : ViewModelBase
{
    private readonly IQueryDispatcher _queries;
    private readonly SessionContext _session;
    private ProjectDto? _selected;

    public ProjectSelectionViewModel(IQueryDispatcher queries, SessionContext session)
    {
        _queries = queries;
        _session = session;
    }

    public ObservableCollection<ProjectDto> Projects { get; } = new();

    public ProjectDto? Selected
    {
        get => _selected;
        set
        {
            if (SetProperty(ref _selected, value) && value is not null)
                _session.SelectProject(value.Id, value.Name);
        }
    }

    /// <summary>
    /// Reloads the project list and reflects the session's current selection in the
    /// picker. The match is compared by id, not by value: <see cref="ProjectDto"/> is a
    /// record whose member collection breaks structural equality, so a freshly loaded
    /// instance of the same project is never "equal" — comparing ids avoids needlessly
    /// re-driving the session selection on every reload.
    /// </summary>
    public async Task LoadAsync()
    {
        Projects.ReplaceAll(await _queries.QueryAsync(new GetAllProjectsQuery()));

        var match = Projects.FirstOrDefault(project => project.Id == _session.SelectedProjectId);
        if (match is not null && match.Id != _selected?.Id)
            Selected = match;
    }
}
