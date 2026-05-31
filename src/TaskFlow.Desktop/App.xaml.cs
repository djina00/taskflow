using System.Windows;

namespace TaskFlow.Desktop;

/// <summary>
/// Interaction logic for App.xaml. On startup it builds the application's
/// dependency-injection container via the <see cref="ServiceConfiguration"/>
/// composition root and exposes it for the UI to resolve dispatchers from.
/// </summary>
public partial class App : Application
{
    /// <summary>The application-wide service provider, available once startup has run.</summary>
    public IServiceProvider Services { get; private set; } = default!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Services = ServiceConfiguration.Build(AppContext.BaseDirectory);
    }
}
