using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Desktop.ViewModels;

namespace TaskFlow.Desktop;

/// <summary>
/// Interaction logic for App.xaml. On startup it builds the DI container, then shows
/// the login window. It observes the shared <see cref="SessionContext"/>: when the
/// user signs in it swaps to the main window, and when they sign out it returns to
/// login. It also moves between the login and register windows in response to the
/// authentication view models' navigation events. In every case it opens the new
/// window before closing the old so the app never briefly drops to zero windows
/// (which would shut it down).
/// </summary>
public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = default!;

    private SessionContext _session = default!;
    private Window? _loginWindow;
    private Window? _registerWindow;
    private Window? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Services = ServiceConfiguration.Build(AppContext.BaseDirectory);

        _session = Services.GetRequiredService<SessionContext>();
        _session.PropertyChanged += OnSessionChanged;

        // The auth view models are singletons, so subscribing once here keeps the
        // login ⇄ register navigation wired for the lifetime of the app.
        Services.GetRequiredService<LoginViewModel>().RegisterRequested += (_, _) => ShowRegister();
        Services.GetRequiredService<RegisterViewModel>().LoginRequested += (_, _) => ShowLogin();

        ShowLogin();
    }

    private void OnSessionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SessionContext.IsLoggedIn))
            return;

        if (_session.IsLoggedIn)
            ShowMain();
        else
            ShowLogin();
    }

    private void ShowMain()
    {
        if (_mainWindow is not null)
            return;

        _mainWindow = Services.GetRequiredService<MainWindow>();
        _mainWindow.Closed += (_, _) => _mainWindow = null;
        _mainWindow.Show();

        CloseWindow(ref _loginWindow);
        CloseWindow(ref _registerWindow);
    }

    private void ShowLogin()
    {
        if (_loginWindow is not null)
            return;

        _loginWindow = Services.GetRequiredService<LoginWindow>();
        _loginWindow.Closed += (_, _) => _loginWindow = null;
        _loginWindow.Show();

        CloseWindow(ref _mainWindow);
        CloseWindow(ref _registerWindow);
    }

    private void ShowRegister()
    {
        if (_registerWindow is not null)
            return;

        _registerWindow = Services.GetRequiredService<RegisterWindow>();
        _registerWindow.Closed += (_, _) => _registerWindow = null;
        _registerWindow.Show();

        CloseWindow(ref _mainWindow);
        CloseWindow(ref _loginWindow);
    }

    private static void CloseWindow(ref Window? window)
    {
        var toClose = window;
        window = null;
        toClose?.Close();
    }
}
