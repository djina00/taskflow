using System.Windows.Input;
using TaskFlow.Desktop.Mvvm;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Root view model bound to the main window. It aggregates the feature view models —
/// one per tab — exposes the shared session for the header, and offers sign-out
/// which clears the session and sends the user back to the login window.
/// </summary>
public sealed class MainViewModel
{
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
    }

    public SessionContext Session { get; }
    public ProjectsViewModel Projects { get; }
    public TasksViewModel Tasks { get; }
    public NotificationsViewModel Notifications { get; }
    public ReportsViewModel Reports { get; }

    public ICommand SignOutCommand { get; }
}
