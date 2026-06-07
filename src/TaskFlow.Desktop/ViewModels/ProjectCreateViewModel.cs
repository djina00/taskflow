using TaskFlow.Desktop.Mvvm;
using TaskFlow.Desktop.Services;
using TaskFlow.Modules.Projects.Application.Commands.CreateProject;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Backs the "new project" dialog opened from the Projects list. It collects the name
/// and description, creates the project (owned by the signed-in user) and raises
/// <see cref="CloseRequested"/> with <c>true</c> on success so the list refreshes.
/// </summary>
public sealed class ProjectCreateViewModel : ViewModelBase, IDialogViewModel
{
    private readonly ICommandDispatcher _commands;
    private readonly SessionContext _session;

    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _status = string.Empty;

    public ProjectCreateViewModel(ICommandDispatcher commands, SessionContext session)
    {
        _commands = commands;
        _session = session;

        CreateCommand = new AsyncRelayCommand(CreateAsync, () => !string.IsNullOrWhiteSpace(Name));
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }

    public event Action<bool>? CloseRequested;
    public string DialogTitle => "New project";

    public string Name
    {
        get => _name;
        set { if (SetProperty(ref _name, value)) CreateCommand.RaiseCanExecuteChanged(); }
    }

    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public IRelayCommand CreateCommand { get; }
    public IRelayCommand CloseCommand { get; }

    private async Task CreateAsync()
    {
        if (_session.CurrentUserId is not Guid ownerId)
        {
            Status = "Sign in first.";
            return;
        }

        var result = await _commands.SendAsync(new CreateProjectCommand(Name, ownerId, Description));
        if (result.IsFailure)
        {
            Status = result.Error.Message;
            return;
        }

        CloseRequested?.Invoke(true);
    }
}
