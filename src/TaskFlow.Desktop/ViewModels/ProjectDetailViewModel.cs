using TaskFlow.Desktop.Mvvm;
using TaskFlow.Desktop.Services;
using TaskFlow.Modules.Projects.Application.Commands.UpdateProject;
using TaskFlow.Modules.Projects.Application.Contracts;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Backs the project detail dialog opened from the Projects list: edit the name and
/// description. It raises <see cref="CloseRequested"/> with <c>true</c> once a change
/// is committed so the list refreshes. (Deletion lives on the project's row in the
/// list, not here.)
/// </summary>
public sealed class ProjectDetailViewModel : ViewModelBase, IDialogViewModel
{
    private readonly ProjectDto _project;
    private readonly ICommandDispatcher _commands;

    private string _name;
    private string _description;
    private string _status = string.Empty;

    public ProjectDetailViewModel(ProjectDto project, ICommandDispatcher commands)
    {
        _project = project;
        _commands = commands;
        _name = project.Name;
        _description = project.Description;

        SaveCommand = new AsyncRelayCommand(SaveAsync, () => !string.IsNullOrWhiteSpace(Name));
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }

    public event Action<bool>? CloseRequested;
    public string DialogTitle => $"Edit project — {_project.Name}";

    public string Name
    {
        get => _name;
        set { if (SetProperty(ref _name, value)) SaveCommand.RaiseCanExecuteChanged(); }
    }

    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CloseCommand { get; }

    private async Task SaveAsync()
    {
        var result = await _commands.SendAsync(new UpdateProjectCommand(_project.Id, Name, Description));
        if (result.IsFailure)
        {
            Status = result.Error.Message;
            return;
        }

        CloseRequested?.Invoke(true);
    }
}
