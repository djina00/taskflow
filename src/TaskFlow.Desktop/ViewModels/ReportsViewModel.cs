using TaskFlow.Desktop.Mvvm;
using TaskFlow.Modules.Reports.Application.Contracts;
using TaskFlow.Modules.Reports.Application.Queries.GetProjectStatistics;
using TaskFlow.Modules.Reports.Application.Queries.GetUserProductivity;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the Reports tab: project statistics for the selected project and the
/// signed-in user's productivity. The data is computed by the Reports module, which
/// reads the Tasks store through its port — the read side stays decoupled from Tasks.
/// </summary>
public sealed class ReportsViewModel : FeatureViewModelBase
{
    private readonly IQueryDispatcher _queries;

    private ProjectReportDto? _projectReport;
    private UserProductivityDto? _userProductivity;

    public ReportsViewModel(IQueryDispatcher queries, SessionContext session,
        ProjectSelectionViewModel projectSelection)
        : base(session)
    {
        _queries = queries;
        ProjectSelection = projectSelection;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        // The view binds the card headers to Session.* (selected project / user), and
        // the figures depend on the selection, so reload the reports whenever it changes.
        Session.PropertyChanged += async (_, _) =>
        {
            OnPropertyChanged(nameof(Session));
            await RefreshAsync();
        };
    }

    /// <summary>The shared project chooser shown at the top of the page.</summary>
    public ProjectSelectionViewModel ProjectSelection { get; }

    public ProjectReportDto? ProjectReport { get => _projectReport; private set => SetProperty(ref _projectReport, value); }
    public UserProductivityDto? UserProductivity { get => _userProductivity; private set => SetProperty(ref _userProductivity, value); }

    public IRelayCommand RefreshCommand { get; }

    public override async Task OnActivatedAsync()
    {
        await ProjectSelection.LoadAsync();
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        ProjectReport = Session.SelectedProjectId is Guid projectId
            ? await _queries.QueryAsync(new GetProjectStatisticsQuery(projectId))
            : null;

        UserProductivity = Session.CurrentUserId is Guid userId
            ? await _queries.QueryAsync(new GetUserProductivityQuery(userId))
            : null;

        Status = "Updated.";
    }
}
