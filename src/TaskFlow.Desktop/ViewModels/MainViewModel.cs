using System.Windows.Input;
using TaskFlow.Desktop.Mvvm;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Root view model bound to the main window. It aggregates the feature view models —
/// one per section of the sidebar — exposes the shared session for the header, drives
/// navigation by surfacing the active feature as <see cref="CurrentViewModel"/>, and
/// offers sign-out which clears the session and returns to the login window.
/// </summary>
public sealed class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private string _activeSection = string.Empty;

    public MainViewModel(
        SessionContext session,
        ProjectsViewModel projects,
        TasksViewModel tasks,
        NotificationsViewModel notifications,
        ReportsViewModel reports)
    {
        Session = session;
        Projects = projects;
        Tasks = tasks;
        Notifications = notifications;
        Reports = reports;

        SignOutCommand = new RelayCommand(session.SignOut);
        NavigateCommand = new RelayCommand<string>(Navigate);

        // This singleton's constructor runs once (on the first sign-in, after which the
        // ctor Navigate below loads the landing section). For every later sign-in, reset
        // to the landing section so its data reloads fresh for the new session.
        Session.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SessionContext.IsLoggedIn) && Session.IsLoggedIn)
            {
                Navigate("Projects");
                Activate(Notifications);
            }
        };

        _currentViewModel = projects;
        Navigate("Projects");
        // Load the unread count up front so the sidebar badge is populated even while
        // the user is on another section and has not opened Notifications yet.
        Activate(Notifications);
    }

    public SessionContext Session { get; }
    public ProjectsViewModel Projects { get; }
    public TasksViewModel Tasks { get; }
    public NotificationsViewModel Notifications { get; }
    public ReportsViewModel Reports { get; }

    /// <summary>The feature view model currently shown in the content region.</summary>
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    /// <summary>The name of the active section, used by the sidebar to highlight its button.</summary>
    public string ActiveSection
    {
        get => _activeSection;
        private set => SetProperty(ref _activeSection, value);
    }

    public ICommand SignOutCommand { get; }
    public ICommand NavigateCommand { get; }

    private void Navigate(string section)
    {
        FeatureViewModelBase target = section switch
        {
            "Tasks" => Tasks,
            "Notifications" => Notifications,
            "Reports" => Reports,
            _ => Projects,
        };
        CurrentViewModel = target;
        ActiveSection = section;
        Activate(target);
    }

    // Loads the section's data when it is shown. Best-effort and fire-and-forget: a
    // failed load must not crash the shell — each feature surfaces errors via its Status.
    private static async void Activate(FeatureViewModelBase viewModel)
    {
        try
        {
            await viewModel.OnActivatedAsync();
        }
        catch
        {
            // Activation is a convenience refresh; ignore failures here.
        }
    }
}
