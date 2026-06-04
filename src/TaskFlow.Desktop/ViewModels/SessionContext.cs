using TaskFlow.Desktop.Mvvm;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Shared UI state passed between the feature view models: who is logged in and
/// which project is currently selected. A single instance is registered in the
/// container so every tab observes the same session — logging in on the Account tab
/// immediately unlocks the others.
/// </summary>
public sealed class SessionContext : ViewModelBase
{
    private Guid? _currentUserId;
    private string _currentUserName = string.Empty;
    private Guid? _selectedProjectId;
    private string _selectedProjectName = string.Empty;

    public Guid? CurrentUserId
    {
        get => _currentUserId;
        private set
        {
            if (SetProperty(ref _currentUserId, value))
                OnPropertyChanged(nameof(IsLoggedIn));
        }
    }

    public string CurrentUserName
    {
        get => _currentUserName;
        private set => SetProperty(ref _currentUserName, value);
    }

    public bool IsLoggedIn => _currentUserId is not null;

    public Guid? SelectedProjectId
    {
        get => _selectedProjectId;
        private set
        {
            if (SetProperty(ref _selectedProjectId, value))
                OnPropertyChanged(nameof(HasSelectedProject));
        }
    }

    public string SelectedProjectName
    {
        get => _selectedProjectName;
        private set => SetProperty(ref _selectedProjectName, value);
    }

    public bool HasSelectedProject => _selectedProjectId is not null;

    public void SignIn(Guid userId, string userName)
    {
        CurrentUserId = userId;
        CurrentUserName = userName;
    }

    public void SelectProject(Guid projectId, string projectName)
    {
        SelectedProjectId = projectId;
        SelectedProjectName = projectName;
    }

    public void SignOut()
    {
        SelectedProjectId = null;
        SelectedProjectName = string.Empty;
        CurrentUserName = string.Empty;
        CurrentUserId = null;   // raises IsLoggedIn last, so observers see a fully cleared session
    }
}
