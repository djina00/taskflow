using System.Windows.Input;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Modules.Users.Application.Commands.LoginUser;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the login window — the authentication gate shown before the main app.
/// On a successful sign-in it calls <see cref="SessionContext.SignIn"/>; the
/// application shell observes that and swaps the login window for the main window.
/// Users without an account navigate to the register window via
/// <see cref="GoToRegisterCommand"/>, which raises <see cref="RegisterRequested"/>.
/// </summary>
public sealed class LoginViewModel : ViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly SessionContext _session;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _status = string.Empty;

    public LoginViewModel(ICommandDispatcher commands, SessionContext session)
    {
        _commands = commands;
        _session = session;

        LoginCommand = new AsyncRelayCommand(LoginAsync);
        GoToRegisterCommand = new RelayCommand(() =>
        {
            Status = string.Empty;
            RegisterRequested?.Invoke(this, EventArgs.Empty);
        });
    }

    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    /// <summary>Raised when the user chooses to create a new account instead of signing in.</summary>
    public event EventHandler? RegisterRequested;

    private async Task LoginAsync()
    {
        var result = await _commands.SendAsync(new LoginUserCommand(Email, Password));
        if (result.IsFailure)
        {
            Status = result.Error.Message;
            return;
        }

        ResetForm();
        _session.SignIn(result.Value.Id, result.Value.FullName);
    }

    private void ResetForm()
    {
        Password = string.Empty;
        Status = string.Empty;
    }
}
