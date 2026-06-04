using System.Windows.Input;
using TaskFlow.Desktop.Mvvm;
using TaskFlow.Modules.Users.Application.Commands.LoginUser;
using TaskFlow.Modules.Users.Application.Commands.RegisterUser;
using TaskFlow.SharedKernel.Messaging;

namespace TaskFlow.Desktop.ViewModels;

/// <summary>
/// Drives the register window. Creating an account signs the new user straight in:
/// on success it calls <see cref="SessionContext.SignIn"/> and the application shell
/// swaps to the main window. Users who already have an account navigate back to the
/// login window via <see cref="GoToLoginCommand"/>, which raises <see cref="LoginRequested"/>.
/// </summary>
public sealed class RegisterViewModel : ViewModelBase
{
    private readonly ICommandDispatcher _commands;
    private readonly SessionContext _session;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _fullName = string.Empty;
    private string _status = string.Empty;

    public RegisterViewModel(ICommandDispatcher commands, SessionContext session)
    {
        _commands = commands;
        _session = session;

        RegisterCommand = new AsyncRelayCommand(RegisterAsync);
        GoToLoginCommand = new RelayCommand(() =>
        {
            Status = string.Empty;
            LoginRequested?.Invoke(this, EventArgs.Empty);
        });
    }

    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    /// <summary>Raised when the user chooses to sign in with an existing account instead.</summary>
    public event EventHandler? LoginRequested;

    private async Task RegisterAsync()
    {
        var register = await _commands.SendAsync(new RegisterUserCommand(Email, Password, FullName));
        if (register.IsFailure)
        {
            Status = register.Error.Message;
            return;
        }

        // Newly registered users are signed straight in.
        var login = await _commands.SendAsync(new LoginUserCommand(Email, Password));
        if (login.IsFailure)
        {
            Status = "Account created — please log in.";
            return;
        }

        ResetForm();
        _session.SignIn(login.Value.Id, login.Value.FullName);
    }

    private void ResetForm()
    {
        Password = string.Empty;
        Status = string.Empty;
    }
}
