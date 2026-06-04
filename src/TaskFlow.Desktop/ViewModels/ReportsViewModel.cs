using System.Windows.Input;
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
public sealed class ReportsViewModel : ViewModelBase
{
    private readonly IQueryDispatcher _queries;
    private readonly SessionContext _session;

    private ProjectReportDto? _projectReport;
    private UserProductivityDto? _userProductivity;
    private string _status = string.Empty;

    public ReportsViewModel(IQueryDispatcher queries, SessionContext session)
    {
        _queries = queries;
        _session = session;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        _session.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Session));
    }

    public SessionContext Session => _session;

    public ProjectReportDto? ProjectReport { get => _projectReport; private set => SetProperty(ref _projectReport, value); }
    public UserProductivityDto? UserProductivity { get => _userProductivity; private set => SetProperty(ref _userProductivity, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public ICommand RefreshCommand { get; }

    private async Task RefreshAsync()
    {
        if (_session.SelectedProjectId is Guid projectId)
            ProjectReport = await _queries.QueryAsync(new GetProjectStatisticsQuery(projectId));
        else
            ProjectReport = null;

        if (_session.CurrentUserId is Guid userId)
            UserProductivity = await _queries.QueryAsync(new GetUserProductivityQuery(userId));
        else
            UserProductivity = null;

        Status = "Updated.";
    }
}
